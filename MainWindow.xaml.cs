using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using BatteryInfo;

namespace BatteryMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        WindowsTheme currentTheme;
        int currentBatterieIndex;

        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            InitializeComponent();

            ThemeSettings.SetThemeData("pack://application:,,,/BatteryMonitor;component/", "DarkModeColors.xaml", "LightModeColors.xaml", "Styles.xaml");
            SetTheme(ThemeSettings.GetWindowsTheme());

            if (UpdateSystemInfo())
            {
                var batteryIndexes = BatteryInfo.BatteryInfo.GetSystemBatteryIndexes();

                if (batteryIndexes.Count > 0)
                {
                    currentBatterieIndex = batteryIndexes[0];
                    UpdateOneBatteryInfo(currentBatterieIndex);

                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(10);
                    timer.Tick += TimerTickHandler;
                    timer.Start();
                }
            }
#if DEBUG
            else
            {
                ShowFakeData();
            }
#endif
        }

#if DEBUG
        private void ShowFakeData()
        {
            // system

            SystemPowerState.Value = ConvertSystemPowerState(BatteryChargeStatus.High);
            SystemChargeState.Value = ConvertSystemChargeState(BatteryChargeStatus.Charging, System.Windows.Forms.PowerLineStatus.Online);
            SystemPowerLineStatus.Value = ConvertSystemPowerLineStatus(System.Windows.Forms.PowerLineStatus.Offline);
            RemainingSystemTime.Value = TimeSpan.FromSeconds(2 * 60 * 60 + 34 * 60 + 12).ToString();
            SystemCapacity.Value = "78";

            // battery

            DeviceName.Value = "DELL 0FDRT85";
            Manufacture.Value = "SMP";
            Chemistry.Value = ConvertChemistry("LiP");
            ManufactureDate.Value = new DateTime(2021, 2, 17).ToString(Properties.Resources.FormatDate);

            DesignedCapacity.Value = "49768";
            CurrentCapacity.Value = "24512";
            CurrentCapacityPercent.Value = "67";
            FullChargeCapacity.Value = "32698";
            BatteryHealth.Value = "67";
            Voltage.Value = "7,568";
            EstimatedTime.Value = TimeSpan.FromSeconds(1 * 60 * 60 + 25 * 60 + 27).ToString();

            Rate.Value = "6254";
            DefaultAlert1.Value = "3241";
            DefaultAlert2.Value = "1254";
            CriticalBias.Value = "123";
            ChargeState.Value = ConvertBatteryChargeState(PowerStates.Critical);
            PowerState.Value = ConvertBatteryPowerState(PowerStates.Discharging);
            PowerLineState.Value = ConvertBatteryPowerLineState(0);
            CylceCount.Value = "546";
            Temperature.Value = "37";
        }
#endif
        private void TimerTickHandler(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            if (!UpdateSystemInfo())
            {
                return;
            }

            UpdateOneBatteryInfo(currentBatterieIndex);
        }

        private bool UpdateSystemInfo()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;

            SystemPowerState.Value = ConvertSystemPowerState(pwr.BatteryChargeStatus);
            SystemChargeState.Value = ConvertSystemChargeState(pwr.BatteryChargeStatus, pwr.PowerLineStatus);
            SystemPowerLineStatus.Value = ConvertSystemPowerLineStatus(pwr.PowerLineStatus);

            if ((pwr.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return false;
            }

            if (pwr.BatteryLifeRemaining > 0)
            {
                RemainingSystemTime.Value = TimeSpan.FromSeconds(pwr.BatteryLifeRemaining).ToString();
            }
            else
            {
                RemainingSystemTime.Value = string.Empty;
            }

            SystemCapacity.Value = (pwr.BatteryLifePercent * 100.0).ToString("F0");

            return true;
        }

        private string ConvertSystemPowerLineStatus(System.Windows.Forms.PowerLineStatus powerLineStatus)
        {
            if (powerLineStatus == System.Windows.Forms.PowerLineStatus.Online)
            {
                return Properties.Resources.PowerLineStateOnline;
            }

            if (powerLineStatus == System.Windows.Forms.PowerLineStatus.Offline)
            {
                return Properties.Resources.PowerLineStateOffline;
            }

            return Properties.Resources.ChargeStatusUnknown;
        }

        private string ConvertSystemPowerState(BatteryChargeStatus powerState)
        {
            if (powerState == BatteryChargeStatus.Unknown)
            {
                return Properties.Resources.ChargeStatusUnknown;
            }

            if ((powerState & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return string.Empty;
            }

            if ((powerState & BatteryChargeStatus.Low) != 0)
            {
                return Properties.Resources.ChargeStatusLow;
            }

            if ((powerState & BatteryChargeStatus.High) != 0)
            {
                return Properties.Resources.ChargeStatusHigh;
            }

            if ((powerState & BatteryChargeStatus.Critical) != 0)
            {
                return Properties.Resources.ChargeStatusCritical;
            }

            return Properties.Resources.ChargeStatusOk;
        }

        private string ConvertSystemChargeState(BatteryChargeStatus powerState, System.Windows.Forms.PowerLineStatus powerLineStatus)
        {
            if (powerState == BatteryChargeStatus.Unknown)
            {
                return Properties.Resources.ChargeStatusUnknown;
            }

            if ((powerState & BatteryChargeStatus.Charging) != 0)
            {
                return Properties.Resources.ChargeStatusCharging;
            }

            if (powerLineStatus == System.Windows.Forms.PowerLineStatus.Offline)
            {
                return Properties.Resources.PowerStateDischarging;
            }

            return Properties.Resources.PowerStateNotCharging;
        }


        private void UpdateOneBatteryInfo(int batteryIndex)
        {
            var battery = BatteryInfo.BatteryInfo.GetBatteryInfo(batteryIndex);

            if (battery == null)
            {
                return;
            }

            try
            {
                DeviceName.Value = battery.DeviceName;
                Manufacture.Value = battery.ManufactureName;
                Chemistry.Value = ConvertChemistry(battery.Chemistry);

                if (battery.ManufactureDate != DateTime.MinValue)
                {
                    ManufactureDate.Value = battery.ManufactureDate.ToString(Properties.Resources.FormatDate);
                }

                DesignedCapacity.Value = battery.DesignedMaxCapacity.ToString();
                CurrentCapacity.Value = battery.CurrentCapacity.ToString();
                CurrentCapacityPercent.Value = ((battery.CurrentCapacity * 100.0) / battery.FullChargedCapacity).ToString("F0");
                FullChargeCapacity.Value = battery.FullChargedCapacity.ToString();
                BatteryHealth.Value = ((battery.FullChargedCapacity * 100.0) / battery.DesignedMaxCapacity).ToString("F0");
                Voltage.Value = ((double)battery.Voltage / 1000.0).ToString();

                if (battery.EstimatedTime == TimeSpan.Zero)
                {
                    EstimatedTime.Value = string.Empty;
                }
                else
                {
                    EstimatedTime.Value = battery.EstimatedTime.ToString();
                }

                Rate.Value = battery.DischargeRate.ToString();
                DefaultAlert1.Value = battery.DefaultAlert1.ToString();
                DefaultAlert2.Value = battery.DefaultAlert2.ToString();
                CriticalBias.Value = battery.CriticalBias.ToString();
                ChargeState.Value = ConvertBatteryChargeState(battery.PowerState);
                PowerState.Value = ConvertBatteryPowerState(battery.PowerState);
                PowerLineState.Value = ConvertBatteryPowerLineState(battery.PowerState);
                CylceCount.Value = battery.CycleCount.ToString();

                if (!double.IsNaN(battery.Temperature))
                {
                    Temperature.Value = battery.Temperature.ToString();
                }
                else
                {
                    Temperature.Value = string.Empty;
                }

            }
            catch (Exception ex)
            {
                Error.Content = ex.ToString();
                Error.Visibility = Visibility.Visible;
            }
        }

        private string ConvertBatteryPowerLineState(PowerStates powerState)
        {
            if ((powerState & PowerStates.PowerOnline) != 0)
            {
                return Properties.Resources.PowerStatePluggedIn;
            }

            return Properties.Resources.PowerLineStateOffline;
        }

        private string ConvertBatteryPowerState(PowerStates powerState)
        {
            if ((powerState & PowerStates.Charging) != 0)
            {
                return Properties.Resources.PowerStateCharging;
            }

            if ((powerState & PowerStates.Discharging) != 0)
            {
                return Properties.Resources.PowerStateDischarging;
            }

            return Properties.Resources.PowerStateNotCharging;
        }

        private string ConvertBatteryChargeState(PowerStates powerState)
        {
            if ((powerState & PowerStates.Critical) != 0)
            {
                return Properties.Resources.PowerStateCritical;
            }

            return Properties.Resources.PowerStateNormal;
        }

        private string ConvertChemistry(string chemistry)
        {
            switch (chemistry)
            {
                case "PbAc":
                    return Properties.Resources.ChemistryPbAc;
                case "LiP":
                    return Properties.Resources.ChemistryLiPo;
                case "Li-I":
                    return Properties.Resources.ChemistryLiIo;
                case "LION":
                    return Properties.Resources.ChemistryLiIo;
                case "NiCd":
                    return Properties.Resources.ChemistryNiCd;
                case "NiMH":
                    return Properties.Resources.ChemistryNiMH;
                case "NiZn":
                    return Properties.Resources.ChemistryNiZn;
                case "RAM":
                    return Properties.Resources.ChemistryRAM;
                default:
                    return chemistry;
            }
        }

        //////////////////////////////////////////////

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            SetTheme(currentTheme == WindowsTheme.Light ? WindowsTheme.Dark : WindowsTheme.Light);
        }

        private void SetTheme(WindowsTheme theme)
        {
            currentTheme = theme;
            var dark = theme == WindowsTheme.Dark;

            if (dark)
            {
                Sonne.Visibility = Visibility.Visible;
                Mond.Visibility = Visibility.Collapsed;
            }
            else
            {
                Sonne.Visibility = Visibility.Collapsed;
                Mond.Visibility = Visibility.Visible;
            }

            ThemeSettings.SetTheme(this, dark);
        }

        private void Button_Click_Test1(object sender, RoutedEventArgs e)
        {
            Test();
            Error.Visibility = Visibility.Visible;
        }

        private void Button_Click_Test2(object sender, RoutedEventArgs e)
        {
            UpdateAll();
        }

        private void Test()
        {
            StringBuilder sb = new StringBuilder();

            Error.Content = sb.ToString();
        }
    }
}
