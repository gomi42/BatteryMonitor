using System;
using System.Collections.Generic;
using Forms = System.Windows.Forms;

namespace BatteryMonitor
{
    internal class DesignViewModel
    {
#if DEBUG
        public DesignViewModel()
        {
            SystemPower = CreateDesignSystemPowerViewModel();
            Batteries = CreateDesignBatteriesViewModel();
            Error = new ErrorViewModel();
        }

        public SystemPowerViewModel SystemPower { get; }
        public List<BatteryViewModel> Batteries { get; }
        public ErrorViewModel Error { get; }

        public static SystemPowerViewModel CreateDesignSystemPowerViewModel()
        {
            var systemPower = new SystemPowerViewModel();
            systemPower.SetPowerStatus(Forms.BatteryChargeStatus.Charging | Forms.BatteryChargeStatus.High,
                                        Forms.PowerLineStatus.Offline,
                                        2 * 60 * 60 + 35 * 60 + 47,
                                        0.68f);
            return systemPower;
        }

        public static List<BatteryViewModel> CreateDesignBatteriesViewModel()
        {
            var batteries = new List<BatteryViewModel>();

            for (int index = 1; index <= 2; index++)
            {
                BatteryData data = new BatteryData(index,
                               true,
                               $"DELL 0FDRT47{index}",
                               $"Company {index}",
                               new DateTime(2021, 2, 17 + index),
                               index % 2 == 0 ? "Lithium Polymer" : "Lithium Ion",
                               49657 + index,
                               32698 + 100 * index,
                               24512 + 100 * index,
                               7568 + 100 * index,
                               TimeSpan.FromSeconds(1 * 60 * 60 + 25 * 60 + 27),
                               6254,
                               546,
                               3241,
                               1254,
                               123,
                               PowerStates.Discharging,
                               37);
                batteries.Add(new BatteryViewModel(index, data));
            }

            return batteries;
        }
#endif
    }
}
