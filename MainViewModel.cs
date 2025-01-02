using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace BatteryMonitor
{
    internal class MainViewModel : ViewModelBase
    {
        DispatcherTimer timer;

        public MainViewModel()
        {
            SystemPower = new SystemPowerViewModel();
            Batteries = new List<BatteryViewModel>();
            Error = new ErrorViewModel();

            if (UpdateSystemPower())
            {
                InitBatteries();

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += TimerTickHandler;
                timer.Start();
            }
#if DEBUG
            else
            {
                // to see some data on none-laptops

                var fake = new DesignViewModel();
                SystemPower = fake.SystemPower;
                Batteries = fake.Batteries;
            }
#endif
        }

        public SystemPowerViewModel SystemPower { get; }
        public List<BatteryViewModel> Batteries { get; }
        public ErrorViewModel Error { get; }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            try
            {
                if (!UpdateSystemPower())
                {
                    return;
                }

                UpdateBatteries();
            }
            catch (Exception ex)
            {
                Error.Text = ex.ToString();
            }
        }

        private bool UpdateSystemPower()
        {
            Forms.PowerStatus pwr = Forms.SystemInformation.PowerStatus;
            return SystemPower.SetPowerStatus(pwr);
        }

        private void InitBatteries()
        {
            List<BatteryData> batteries;

            try
            {
                batteries = BatteryInformation.GetAllSystemBatteries();

                if (batteries == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Error.Text = ex.ToString();
                return;
            }

            if (batteries.Count == 1)
            {
                Batteries.Add(new BatteryViewModel(batteries[0]));
            }
            else
            {
                int index = 1;

                foreach (var battery in batteries)
                {
                    Batteries.Add(new BatteryViewModel(index, battery));
                    index++;
                }
            }
        }

        private void UpdateBatteries()
        {
            var batteries = BatteryInformation.GetAllSystemBatteries();

            if (batteries == null)
            {
                return;
            }

            for (int i = 0; i < Batteries.Count; i++)
            {
                Batteries[i].SetBattery(batteries[i]);
            }
        }
    }
}
