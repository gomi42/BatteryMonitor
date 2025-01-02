using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace BatteryMonitor
{
    internal class MainViewModel : ViewModelBase
    {
        DispatcherTimer timer;
        int currentBatterieIndex;

        public MainViewModel()
        {
            System = new SystemPowerViewModel();
            Battery = new BatteryViewModel();
            Error = new ErrorViewModel();

            if (UpdateSystemInfo())
            {
                var batteryIndexes = BatteryInformation.GetSystemBatteryIndexes();

                if (batteryIndexes.Count > 0)
                {
                    currentBatterieIndex = batteryIndexes[0];
                    UpdateOneBattery(currentBatterieIndex);

                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(10);
                    timer.Tick += TimerTickHandler;
                    timer.Start();
                }
            }
#if DEBUG
            else
            {
                // to see some data on none-laptops

                var fake = new DesignViewModel();
                System = fake.System;
                Battery = fake.Battery;
            }
#endif
        }

        public SystemPowerViewModel System { get; }
        public BatteryViewModel Battery { get; }
        public ErrorViewModel Error { get; }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            try
            {
                if (!UpdateSystemInfo())
                {
                    return;
                }

                UpdateOneBattery(currentBatterieIndex);
            }
            catch (Exception ex)
            {
                Error.Text = ex.ToString();
            }
        }

        private bool UpdateSystemInfo()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;
            return System.SetPowerStatus(pwr);
        }


        private void UpdateOneBattery(int batteryIndex)
        {
            var battery = BatteryInformation.GetBattery(batteryIndex);

            if (battery == null)
            {
                return;
            }

            Battery.SetBattery(battery);
        }

    }
}
