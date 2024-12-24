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
        WindowsTheme theme;

        public MainWindow()
        {
            InitializeComponent();

            ThemeSettings.SetThemeData("pack://application:,,,/BatteryMonitor;component/", "DarkModeColors.xaml", "LightModeColors.xaml", "Styles.xaml");
            theme = ThemeSettings.GetWindowsTheme();
            SetTheme();

            if (UpdateAll())
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += TimerTickHandler;
                timer.Start();
            }
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private bool UpdateAll()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;

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

            try
            {
                var batteries = BatteryInfo.BatteryInfo.GetBatteryData();

                if (batteries.Count == 0)
                {
                    return false;
                }

                var battery = batteries[0];

                DeviceName.Content = battery.DeviceName;
                Manufacture.Content = battery.ManufactureName;
                Chemistry.Content = ConvertChemistry(battery.Chemistry);

                if (battery.ManufactureDate != DateTime.MinValue)
                {
                    ManufactureDate.Content = battery.ManufactureDate.ToString("d.m.yyyy");
                }

                DesignedCapacity.Content = battery.DesignedMaxCapacity.ToString();
                CurrentCapacity.Content = battery.CurrentCapacity.ToString();
                CurrentCapacityPercent.Content = ((battery.CurrentCapacity * 100.0) / battery.FullChargeCapacity).ToString("F0");
                FullChargeCapacity.Content = battery.FullChargeCapacity.ToString();
                BatteryHealth.Content = ((int)Math.Round((battery.FullChargeCapacity * 100.0) / (uint)battery.DesignedMaxCapacity)).ToString();
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

            return true;
        }

        private string ConvertPowerState(PowerStates powerState)
        {
            List<string> states = new List<string>();

            if ((powerState & PowerStates.PowerOnline) != 0)
                states.Add("eingesteckt");

            if ((powerState & PowerStates.Charging) != 0)
                states.Add("wird geladen");

            if ((powerState & PowerStates.Discharging) != 0)
                states.Add("wird entladen");

            if ((powerState & PowerStates.Critical) != 0)
                states.Add("kritisch");

            return string.Join(", ", states);
        }

        private string ConvertChemistry(string chemistry)
        {
            switch (chemistry)
            {
                case "LiP": return "Lithium-Polymer";
                case "Li-I": return "Lithium-Ion";
                case "LÖWE": return "Litium-Ion";
                case "NiCd": return "Nickel-Cadmium";
                case "NiMH": return "Nickel-Metallhydrid";
                case "NiZn": return "Nickel-Zink";
                case "RAM": return "aufladbare Alkali-Mangan";
                default: return chemistry;
            }
        }

        //////////////////////////////////////////////

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (theme == WindowsTheme.Light)
            {
                theme = WindowsTheme.Dark;
            }
            else
            {
                theme = WindowsTheme.Light;
            }

            SetTheme();
        }

        private void SetTheme()
        {
            var dark = theme == WindowsTheme.Dark;

            if (dark)
            {
                Sonne.Visibility = Visibility.Visible;
                Monde.Visibility = Visibility.Collapsed;
            }
            else
            {
                Sonne.Visibility = Visibility.Collapsed;
                Monde.Visibility = Visibility.Visible;
            }

            ThemeSettings.SetTheme(this, dark);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Test();
            Error.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateAll();
        }

        private void Test()
        {
            StringBuilder sb = new StringBuilder();

            PowerStatus pwr = SystemInformation.PowerStatus;

            sb.AppendLine(pwr.BatteryLifePercent.ToString());
            sb.AppendLine(TimeSpan.FromSeconds(pwr.BatteryLifeRemaining).ToString());
            sb.AppendLine(TimeSpan.FromSeconds(pwr.BatteryFullLifetime).ToString());
            Error.Content = sb.ToString();
        }
    }
}
