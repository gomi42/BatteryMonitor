using System;
using System.Collections.Generic;
using BatteryMonitor.Properties;

namespace BatteryMonitor
{
#if DEBUG
    internal class DesignSystemPowerViewModel : ISystemPowerViewModel
    {
        public string PowerState { get; set; }

        public string ChargeState { get; set; }

        public string PowerLineStatus { get; set; }

        public string RemainingTime { get; set; }

        public string Capacity { get; set; }
    }

    class DesignBatteryViewModel : IBatteryViewModel
    {
        public string Index { get; set; }

        public string DeviceName { get; set; }

        public string Manufacture { get; set; }

        public string Chemistry { get; set; }

        public string ManufactureDate { get; set; }

        public string DesignedCapacity { get; set; }

        public string CurrentCapacity { get; set; }

        public string CurrentCapacityPercent { get; set; }

        public string FullChargeCapacity { get; set; }

        public string BatteryHealth { get; set; }

        public string Voltage { get; set; }

        public string EstimatedTime { get; set; }

        public string Rate { get; set; }

        public string DefaultAlert1 { get; set; }

        public string DefaultAlert2 { get; set; }

        public string CriticalBias { get; set; }

        public string ChargeState { get; set; }

        public string PowerState { get; set; }

        public string PowerLineState { get; set; }

        public string CylceCount { get; set; }

        public string Temperature { get; set; }
    }
#endif

    internal class DesignViewModel
    {
#if DEBUG
        public DesignViewModel()
        {
            SystemPower = CreateDesignSystemPowerViewModel();
            Batteries = CreateDesignBatteriesViewModel();
            Error = new ErrorViewModel();
        }

        public ISystemPowerViewModel SystemPower { get; }
        public List<IBatteryViewModel> Batteries { get; }
        public ErrorViewModel Error { get; }

        public static ISystemPowerViewModel CreateDesignSystemPowerViewModel()
        {
            var systemPower = new DesignSystemPowerViewModel();

            systemPower.PowerState = "hoch";
            systemPower.ChargeState = "wird geladen";
            systemPower.PowerLineStatus = "nicht eingesteckt";
            systemPower.RemainingTime = "02:45:43";
            systemPower.Capacity = "78";

            return systemPower;
        }

        public static List<IBatteryViewModel> CreateDesignBatteriesViewModel()
        {
            var batteries = new List<IBatteryViewModel>();

            for (int i = 1; i <= 2; i++)
            {
                batteries.Add(CreateOneBattery(i));
            }

            return batteries;
        }

        private static IBatteryViewModel CreateOneBattery(int index)
        {
            var battery = new DesignBatteryViewModel();

            battery.Index = index.ToString();
            battery.DeviceName = $"DELL 0FDRT47{index}";
            battery.Manufacture = $"Company {index}";
            battery.Chemistry = index % 2 == 0 ? "Lithium Polymer" : "Lithium Ion";
            battery.ManufactureDate = new DateTime(2021, 2, 17 + index).ToString(Resources.FormatDate);
            battery.DesignedCapacity = (49657 + index).ToString();
            battery.CurrentCapacity = (24512 + index).ToString();
            battery.CurrentCapacityPercent = (67 + index).ToString();
            battery.FullChargeCapacity = (32698 + index).ToString();
            battery.BatteryHealth = (57 + index).ToString();
            battery.Voltage = "7,568";
            battery.EstimatedTime = TimeSpan.FromSeconds(1 * 60 * 60 + 25 * 60 + 27).ToString();
            battery.Rate = "6254";
            battery.DefaultAlert1 = "3241";
            battery.DefaultAlert2 = "1254";
            battery.CriticalBias = "123";
            battery.ChargeState = "kritisch";
            battery.PowerState = "wird geladen";
            battery.PowerLineState = "eingesteckt";
            battery.CylceCount = "546";
            battery.Temperature = "37";

            return battery;
        }
#endif
    }
}
