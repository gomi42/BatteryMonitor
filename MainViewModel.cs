﻿using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace BatteryMonitor
{
    internal class MainViewModel : ViewModelBase, IDisposable
    {
        private DispatcherTimer timer;
        private string error;
        private bool noBatterieFound;

        public MainViewModel()
        {
            SystemPower = new SystemPowerViewModel();
            Batteries = new List<BatteryViewModel>();

            if (UpdateSystemPower())
            {
                InitBatteries();

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += TimerTickHandler;
                timer.Start();

                Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerModeChanged;
            }
            else
            {
                noBatterieFound = true;
#if DEBUG
                // to see some data on none-laptops
                SystemPower = DesignViewModel.CreateDesignSystemPowerViewModel();
                Batteries = DesignViewModel.CreateDesignBatteriesViewModel();
#endif
            }
        }

        public void Dispose()
        {
            if (!noBatterieFound)
            {
                Microsoft.Win32.SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            }
        }

        public SystemPowerViewModel SystemPower { get; }
        public List<BatteryViewModel> Batteries { get; }

        public string Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.StatusChange)
            {
                UpdateAll();
            }
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
                Error = ex.ToString();
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
                Error = ex.ToString();
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
