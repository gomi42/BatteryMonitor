using System;
using BatteryMonitor.Properties;
using Forms = System.Windows.Forms;

namespace BatteryMonitor
{
    internal class SystemPowerViewModel : ViewModelBase, ISystemPower
    {
        private string powerState;
        private string chargeState;
        private string powerLineStatus;
        private string remainingTime;
        private string capacity;

        public string PowerState
        {
            get => powerState;
            private set => SetProperty(ref powerState, value);
        }

        public string ChargeState
        {
            get => chargeState;
            private set => SetProperty(ref chargeState, value);
        }

        public string PowerLineStatus
        {
            get => powerLineStatus;
            private set => SetProperty(ref powerLineStatus, value);
        }

        public string RemainingTime
        {
            get => remainingTime;
            private set => SetProperty(ref remainingTime, value);
        }

        public string Capacity
        {
            get => capacity;
            private set => SetProperty(ref capacity, value);
        }

        public bool SetPowerStatus(Forms.PowerStatus status)
        {
            PowerState = ConvertPowerState(status.BatteryChargeStatus);
            ChargeState = ConvertChargeState(status.BatteryChargeStatus, status.PowerLineStatus);
            PowerLineStatus = ConvertPowerLineStatus(status.PowerLineStatus);

            if ((status.BatteryChargeStatus & Forms.BatteryChargeStatus.NoSystemBattery) != 0)
            {
                RemainingTime = string.Empty;
                Capacity = string.Empty;

                return false;
            }

            if (status.BatteryLifeRemaining > 0)
            {
                RemainingTime = TimeSpan.FromSeconds(status.BatteryLifeRemaining).ToString();
            }
            else
            {
                RemainingTime = string.Empty;
            }

            Capacity = (status.BatteryLifePercent * 100.0).ToString("F0");

            return true;
        }

        private string ConvertPowerLineStatus(Forms.PowerLineStatus powerLineStatus)
        {
            if (powerLineStatus == Forms.PowerLineStatus.Online)
            {
                return Resources.PowerLineStateOnline;
            }

            if (powerLineStatus == Forms.PowerLineStatus.Offline)
            {
                return Resources.PowerLineStateOffline;
            }

            return Resources.ChargeStatusUnknown;
        }

        private string ConvertPowerState(Forms.BatteryChargeStatus powerState)
        {
            if (powerState == Forms.BatteryChargeStatus.Unknown)
            {
                return Resources.ChargeStatusUnknown;
            }

            if ((powerState & Forms.BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return string.Empty;
            }

            if ((powerState & Forms.BatteryChargeStatus.Low) != 0)
            {
                return Resources.ChargeStatusLow;
            }

            if ((powerState & Forms.BatteryChargeStatus.High) != 0)
            {
                return Resources.ChargeStatusHigh;
            }

            if ((powerState & Forms.BatteryChargeStatus.Critical) != 0)
            {
                return Resources.ChargeStatusCritical;
            }

            return Resources.ChargeStatusOk;
        }

        private string ConvertChargeState(Forms.BatteryChargeStatus powerState,
                                                Forms.PowerLineStatus powerLineStatus)
        {
            if (powerState == Forms.BatteryChargeStatus.Unknown)
            {
                return Resources.ChargeStatusUnknown;
            }

            if ((powerState & Forms.BatteryChargeStatus.Charging) != 0)
            {
                return Resources.ChargeStatusCharging;
            }

            if (powerLineStatus == Forms.PowerLineStatus.Offline)
            {
                return Resources.PowerStateDischarging;
            }

            return Resources.PowerStateNotCharging;
        }
    }
}
