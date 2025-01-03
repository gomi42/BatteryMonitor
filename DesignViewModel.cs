using System;
using System.Collections.Generic;
using BatteryMonitor.Properties;

namespace BatteryMonitor
{
#if DEBUG
    internal class DesignSystemPowerViewModel : ISystemPower
    {
        public string PowerState {  get; set; }

        public string ChargeState {  get; set; }

        public string PowerLineStatus {  get; set; }

        public string RemainingTime {  get; set; }

        public string Capacity {  get; set; }
    }
#endif

    internal class DesignViewModel
    {
#if DEBUG
        public DesignViewModel()
        {
            SystemPower = new DesignSystemPowerViewModel();
            Batteries = new List<BatteryViewModel>();
            Error = new ErrorViewModel();

            InitSystem();
            InitBatteries();
        }

        public ISystemPower SystemPower { get; }
        public List<BatteryViewModel> Batteries { get; }
        public ErrorViewModel Error { get; }

        private void InitSystem()
        {
            var systemPower = (DesignSystemPowerViewModel)SystemPower;

            systemPower.PowerState = "hoch";
            systemPower.ChargeState = "wird geladen";
            systemPower.PowerLineStatus = "nicht eingesteckt";
            systemPower.RemainingTime = "02:45:43";
            systemPower.Capacity = "78";
        }

        private void InitBatteries()
        {
            for (int i = 1; i <= 2; i++)
            {
                Batteries.Add(GetOneBattery(i));
            }
        }

        private BatteryViewModel GetOneBattery(int index)
        {
            BatteryViewModel bvm = new BatteryViewModel(index);

            bvm.DeviceName = $"DELL 0FDRT47{index}";
            bvm.Manufacture = $"Company {index}";
            bvm.Chemistry = index % 2 == 0 ? "Lithium Polymer" : "Lithium Ion";
            bvm.ManufactureDate = new DateTime(2021, 2, 17 + index).ToString(Resources.FormatDate);
            bvm.DesignedCapacity = (49657 + index).ToString();
            bvm.CurrentCapacity = (24512 + index).ToString();
            bvm.CurrentCapacityPercent = (67 + index).ToString();
            bvm.FullChargeCapacity = (32698 + index).ToString();
            bvm.BatteryHealth = (57 + index).ToString();
            bvm.Voltage = "7,568";
            bvm.EstimatedTime = TimeSpan.FromSeconds(1 * 60 * 60 + 25 * 60 + 27).ToString();
            bvm.Rate = "6254";
            bvm.DefaultAlert1 = "3241";
            bvm.DefaultAlert2 = "1254";
            bvm.CriticalBias = "123";
            bvm.ChargeState = "kritisch";
            bvm.PowerState = "wird geladen";
            bvm.PowerLineState = "eingesteckt";
            bvm.CylceCount = "546";
            bvm.Temperature = "37";

            return bvm;
        }
#endif
    }
}
