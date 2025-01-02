using System;

namespace BatteryMonitor
{
    internal class DesignViewModel
    {
#if DEBUG
        public DesignViewModel()
        {
            System = new SystemPowerViewModel();
            Battery = new BatteryViewModel();
            Error = new ErrorViewModel();

            InitSystem();
            InitBattery();
        }

        public SystemPowerViewModel System { get; }
        public BatteryViewModel Battery { get; }
        public ErrorViewModel Error { get; }

        private void InitSystem()
        {
            // system

            System.PowerState = "hoch";
            System.ChargeState = "wird geladen";
            System.PowerLineStatus = "nicht eingesteckt";
            System.RemainingTime = "02:45:43";
            System.Capacity = "78";
        }

        private void InitBattery()
        {
            Battery.DeviceName = "DELL 0FDRT85";
            Battery.Manufacture = "SMP";
            Battery.Chemistry = "Lithium Polymer";
            Battery.ManufactureDate = new DateTime(2021, 2, 17).ToString(Properties.Resources.FormatDate);

            Battery.DesignedCapacity = "49768";
            Battery.CurrentCapacity = "24512";
            Battery.CurrentCapacityPercent = "67";
            Battery.FullChargeCapacity = "32698";
            Battery.BatteryHealth = "67";
            Battery.Voltage = "7,568";
            Battery.EstimatedTime = TimeSpan.FromSeconds(1 * 60 * 60 + 25 * 60 + 27).ToString();

            Battery.Rate = "6254";
            Battery.DefaultAlert1 = "3241";
            Battery.DefaultAlert2 = "1254";
            Battery.CriticalBias = "123";
            Battery.ChargeState = "kritisch";
            Battery.PowerState = "wird entladen";
            Battery.PowerLineState = "nicht eingesteckt";
            Battery.CylceCount = "546";
            Battery.Temperature = "37";
        }
#endif
    }
}
