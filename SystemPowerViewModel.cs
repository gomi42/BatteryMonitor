﻿using System;
using BatteryMonitor.Properties;
using Forms = System.Windows.Forms;

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
            return SetPowerStatus(status.BatteryChargeStatus,
                                  status.PowerLineStatus,
                                  status.BatteryLifeRemaining,
                                  status.BatteryLifePercent);
        }

        public bool SetPowerStatus(Forms.BatteryChargeStatus chargeStatus,
                                   Forms.PowerLineStatus powerLineStatus,
                                   int batteryLifeRemaining,
                                   float batteryLifePercent)
        {
            PowerState = ConvertPowerState(chargeStatus);
            ChargeState = ConvertChargeState(chargeStatus, powerLineStatus);
            PowerLineStatus = ConvertPowerLineStatus(powerLineStatus);

            if ((chargeStatus & Forms.BatteryChargeStatus.NoSystemBattery) != 0)
            {
                RemainingTime = string.Empty;
                Capacity = string.Empty;

                return false;
            }

            if (batteryLifeRemaining > 0)
            {
                RemainingTime = TimeSpan.FromSeconds(batteryLifeRemaining).ToString();
            }
            else
            {
                RemainingTime = string.Empty;
            }

            Capacity = (batteryLifePercent * 100.0).ToString("F0");

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
