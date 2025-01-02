using System;
using System.Windows.Forms;
using BatteryMonitor.Properties;

namespace BatteryMonitor
{
    internal class SystemPowerViewModel : ViewModelBase
    {
        private string powerState;
        private string chargeState;
        private string powerLineStatus;
        private string remainingTime;
        private string capacity;

        public string PowerState
        {
            get => powerState;
            set => SetProperty(ref powerState, value);
        }

        public string ChargeState
        {
            get => chargeState;
            set => SetProperty(ref chargeState, value);
        }

        public string PowerLineStatus
        {
            get => powerLineStatus;
            set => SetProperty(ref powerLineStatus, value);
        }

        public string RemainingTime
        {
            get => remainingTime;
            set => SetProperty(ref remainingTime, value);
        }

        public string Capacity
        {
            get => capacity;
            set => SetProperty(ref capacity, value);
        }

        public bool SetPowerStatus(PowerStatus pwr)
        {
            PowerState = ConvertSystemPowerState(pwr.BatteryChargeStatus);
            ChargeState = ConvertSystemChargeState(pwr.BatteryChargeStatus, pwr.PowerLineStatus);
            PowerLineStatus = ConvertSystemPowerLineStatus(pwr.PowerLineStatus);

            if ((pwr.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return false;
            }

            if (pwr.BatteryLifeRemaining > 0)
            {
                RemainingTime = TimeSpan.FromSeconds(pwr.BatteryLifeRemaining).ToString();
            }
            else
            {
                RemainingTime = string.Empty;
            }

            Capacity = (pwr.BatteryLifePercent * 100.0).ToString("F0");

            return true;

        }

        private string ConvertSystemPowerLineStatus(System.Windows.Forms.PowerLineStatus powerLineStatus)
        {
            if (powerLineStatus == global::System.Windows.Forms.PowerLineStatus.Online)
            {
                return Resources.PowerLineStateOnline;
            }

            if (powerLineStatus == global::System.Windows.Forms.PowerLineStatus.Offline)
            {
                return Resources.PowerLineStateOffline;
            }

            return Resources.ChargeStatusUnknown;
        }

        private string ConvertSystemPowerState(BatteryChargeStatus powerState)
        {
            if (powerState == BatteryChargeStatus.Unknown)
            {
                return Resources.ChargeStatusUnknown;
            }

            if ((powerState & BatteryChargeStatus.NoSystemBattery) != 0)
            {
                return string.Empty;
            }

            if ((powerState & BatteryChargeStatus.Low) != 0)
            {
                return Resources.ChargeStatusLow;
            }

            if ((powerState & BatteryChargeStatus.High) != 0)
            {
                return Resources.ChargeStatusHigh;
            }

            if ((powerState & BatteryChargeStatus.Critical) != 0)
            {
                return Resources.ChargeStatusCritical;
            }

            return Resources.ChargeStatusOk;
        }

        private string ConvertSystemChargeState(BatteryChargeStatus powerState,
                                                System.Windows.Forms.PowerLineStatus powerLineStatus)
        {
            if (powerState == BatteryChargeStatus.Unknown)
            {
                return Resources.ChargeStatusUnknown;
            }

            if ((powerState & BatteryChargeStatus.Charging) != 0)
            {
                return Resources.ChargeStatusCharging;
            }

            if (powerLineStatus == global::System.Windows.Forms.PowerLineStatus.Offline)
            {
                return Resources.PowerStateDischarging;
            }

            return Resources.PowerStateNotCharging;
        }
    }
}
