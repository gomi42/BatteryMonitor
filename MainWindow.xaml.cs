using System;
using System.Collections.Generic;
using System.Text;
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
        }

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

            SystemPowerState.Content = ConvertPowerState(pwr.BatteryChargeStatus);

            SystemPowerLineStatus.Content = ConvertPowerLineStatus(pwr.PowerLineStatus);

            if ((pwr.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return false;
            }

            if (pwr.BatteryLifeRemaining > 0)
            {
                RemainingSystemTime.Content = TimeSpan.FromSeconds(pwr.BatteryLifeRemaining).ToString();
            }
            else
            {
                RemainingSystemTime.Content = "--";
            }

            SystemCapacity.Content = (pwr.BatteryLifePercent * 100.0).ToString("F0");

            return true;
        }

        private string ConvertPowerLineStatus(System.Windows.Forms.PowerLineStatus powerLineStatus)
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

        private string ConvertPowerState(BatteryChargeStatus powerState)
        {
            if (powerState == BatteryChargeStatus.Unknown)
            {
                return Properties.Resources.ChargeStatusUnknown;
            }

            List<string> states = new List<string>();
            bool stateFound = false;

            if ((powerState & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return Properties.Resources.ChargeStatusNoSystemBattery;
            }

            if ((powerState & BatteryChargeStatus.Charging) != 0)
                states.Add(Properties.Resources.ChargeStatusCharging);

            if ((powerState & BatteryChargeStatus.Low) != 0)
            {
                stateFound = true;
                states.Add(Properties.Resources.ChargeStatusLow);
            }

            if ((powerState & BatteryChargeStatus.High) != 0)
            {
                stateFound = true;
                states.Add(Properties.Resources.ChargeStatusHigh);
            }

            if ((powerState & BatteryChargeStatus.Critical) != 0)
            {
                stateFound = true;
                states.Add(Properties.Resources.ChargeStatusCritical);
            }

            if (!stateFound)
            {
                states.Add(Properties.Resources.ChargeStatusOk);
            }

            return string.Join(", ", states);
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
                DeviceName.Content = battery.DeviceName;
                Manufacture.Content = battery.ManufactureName;
                Chemistry.Content = ConvertChemistry(battery.Chemistry);

                if (battery.ManufactureDate != DateTime.MinValue)
                {
                    ManufactureDate.Content = battery.ManufactureDate.ToString(Properties.Resources.FormatDate);
                }

                DesignedCapacity.Content = battery.DesignedMaxCapacity.ToString();
                CurrentCapacity.Content = battery.CurrentCapacity.ToString();
                CurrentCapacityPercent.Content = ((battery.CurrentCapacity * 100.0) / battery.FullChargedCapacity).ToString("F0");
                FullChargeCapacity.Content = battery.FullChargedCapacity.ToString();
                BatteryHealth.Content = ((battery.FullChargedCapacity * 100.0) / battery.DesignedMaxCapacity).ToString("F0");
                Voltage.Content = ((double)battery.Voltage / 1000.0).ToString();

                if (battery.EstimatedTime == TimeSpan.Zero)
                {
                    EstimatedTime.Content = "--";
                }
                else
                {
                    EstimatedTime.Content = battery.EstimatedTime.ToString();
                }

                Rate.Content = battery.DischargeRate.ToString();
                DefaultAlert1.Content = battery.DefaultAlert1.ToString();
                DefaultAlert2.Content = battery.DefaultAlert2.ToString();
                CriticalBias.Content = battery.CriticalBias.ToString();
                PowerState.Content = ConvertPowerState(battery.PowerState);
                CylceCount.Content = battery.CycleCount.ToString();

                if (!double.IsNaN(battery.Temperature))
                {
                    Temperature.Content = battery.Temperature.ToString();
                }

            }
            catch (Exception ex)
            {
                Error.Content = ex.ToString();
                Error.Visibility = Visibility.Visible;
            }
        }

        private string ConvertPowerState(PowerStates powerState)
        {
            List<string> states = new List<string>();

            if ((powerState & PowerStates.PowerOnline) != 0)
                states.Add(Properties.Resources.PowerStatePluggedIn);

            if ((powerState & PowerStates.Charging) != 0)
                states.Add(Properties.Resources.PowerStateCharging);

            if ((powerState & PowerStates.Discharging) != 0)
                states.Add(Properties.Resources.PowerStateDischarging);

            if ((powerState & PowerStates.Critical) != 0)
                states.Add(Properties.Resources.PowerStateCritical);

            return string.Join(", ", states);
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

            PowerStatus pwr = SystemInformation.PowerStatus;

            sb.AppendLine(pwr.BatteryChargeStatus.ToString("X"));
            sb.AppendLine(ConvertPowerState(pwr.BatteryChargeStatus));

            sb.AppendLine(pwr.BatteryLifePercent.ToString());
            sb.AppendLine(TimeSpan.FromSeconds(pwr.BatteryLifeRemaining).ToString());
            sb.AppendLine(TimeSpan.FromSeconds(pwr.BatteryFullLifetime).ToString());

            var battery = BatteryInfo.BatteryInfo.GetBatteryInfo(currentBatterieIndex);
            var ps = battery.PowerState.ToString("X");
            sb.AppendLine(ps);

            Error.Content = sb.ToString();
        }
    }
}
