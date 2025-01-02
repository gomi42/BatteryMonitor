using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryMonitor
{
    internal class SystemInfoViewModel : ViewModelBase
    {
        private string systemPowerState;
        private string systemChargeState;
        private string systemPowerLineStatus;
        private string remainingSystemTime;
        private string systemCapacity;

        public string SystemPowerState
        {
            get => systemPowerState;
            set => SetProperty(ref systemPowerState, value);
        }

        public string SystemChargeState
        {
            get => systemChargeState;
            set => SetProperty(ref systemChargeState, value);
        }

        public string SystemPowerLineStatus
        {
            get => systemPowerLineStatus;
            set => SetProperty(ref systemPowerLineStatus, value);
        }

        public string RemainingSystemTime
        {
            get => remainingSystemTime;
            set => SetProperty(ref remainingSystemTime, value);
        }

        public string SystemCapacity
        {
            get => systemCapacity;
            set => SetProperty(ref systemCapacity, value);
        }
    }
}
