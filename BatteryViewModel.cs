using System;
using BatteryMonitor.Properties;

namespace BatteryMonitor
{
    internal class BatteryViewModel : ViewModelBase
    {
        private string index;
        private string deviceName;
        private string manufacture;
        private string chemistry;
        private string manufactureDate;
        private string designedCapacity;
        private string currentCapacity;
        private string currentCapacityPercent;
        private string fullChargeCapacity;
        private string batteryHealth;
        private string voltage;
        private string estimatedTime;
        private string rate;
        private string defaultAlert1;
        private string defaultAlert2;
        private string criticalBias;
        private string chargeState;
        private string powerState;
        private string powerLineState;
        private string cylceCount;
        private string temperature;

        public BatteryViewModel(int index)
        {
            Index = $" {index}";
        }

        public BatteryViewModel(BatteryData battery)
        {
            SetBattery(battery);
        }

        public BatteryViewModel(int index, BatteryData battery)
        {
            if (index > 0)
            {
                Index = $" {index}";
            }

            SetBattery(battery);
        }

        public string Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }

        public string DeviceName
        {
            get => deviceName;
            set => SetProperty(ref deviceName, value);
        }

        public string Manufacture
        {
            get => manufacture;
            set => SetProperty(ref manufacture, value);
        }

        public string Chemistry
        {
            get => chemistry;
            set => SetProperty(ref chemistry, value);
        }

        public string ManufactureDate
        {
            get => manufactureDate;
            set => SetProperty(ref manufactureDate, value);
        }

        public string DesignedCapacity
        {
            get => designedCapacity;
            set => SetProperty(ref designedCapacity, value);
        }

        public string CurrentCapacity
        {
            get => currentCapacity;
            set => SetProperty(ref currentCapacity, value);
        }

        public string CurrentCapacityPercent
        {
            get => currentCapacityPercent;
            set => SetProperty(ref currentCapacityPercent, value);
        }

        public string FullChargeCapacity
        {
            get => fullChargeCapacity;
            set => SetProperty(ref fullChargeCapacity, value);
        }

        public string BatteryHealth
        {
            get => batteryHealth;
            set => SetProperty(ref batteryHealth, value);
        }


        public string Voltage
        {
            get => voltage;
            set => SetProperty(ref voltage, value);
        }

        public string EstimatedTime
        {
            get => estimatedTime;
            set => SetProperty(ref estimatedTime, value);
        }

        public string Rate
        {
            get => rate;
            set => SetProperty(ref rate, value);
        }

        public string DefaultAlert1
        {
            get => defaultAlert1;
            set => SetProperty(ref defaultAlert1, value);
        }

        public string DefaultAlert2
        {
            get => defaultAlert2;
            set => SetProperty(ref defaultAlert2, value);
        }

        public string CriticalBias
        {
            get => criticalBias;
            set => SetProperty(ref criticalBias, value);
        }

        public string ChargeState
        {
            get => chargeState;
            set => SetProperty(ref chargeState, value);
        }


        public string PowerState
        {
            get => powerState;
            set => SetProperty(ref powerState, value);
        }

        public string PowerLineState
        {
            get => powerLineState;
            set => SetProperty(ref powerLineState, value);
        }

        public string CylceCount
        {
            get => cylceCount;
            set => SetProperty(ref cylceCount, value);
        }

        public string Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }

        public void SetBattery(BatteryData battery)
        {
            DeviceName = battery.DeviceName;
            Manufacture = battery.ManufactureName;
            Chemistry = ConvertChemistry(battery.Chemistry);

            if (battery.ManufactureDate != DateTime.MinValue)
            {
                ManufactureDate = battery.ManufactureDate.ToString(Resources.FormatDate);
            }
            else
            {
                ManufactureDate = string.Empty;
            }

            DesignedCapacity = battery.DesignedMaxCapacity.ToString();
            CurrentCapacity = battery.CurrentCapacity.ToString();
            CurrentCapacityPercent = ((battery.CurrentCapacity * 100.0) / battery.FullChargedCapacity).ToString("F0");
            FullChargeCapacity = battery.FullChargedCapacity.ToString();
            BatteryHealth = ((battery.FullChargedCapacity * 100.0) / battery.DesignedMaxCapacity).ToString("F0");
            Voltage = ((double)battery.Voltage / 1000.0).ToString();

            if (battery.EstimatedTime != TimeSpan.Zero)
            {
                EstimatedTime = battery.EstimatedTime.ToString();
            }
            else
            {
                EstimatedTime = string.Empty;
            }

            Rate = battery.DischargeRate.ToString();
            DefaultAlert1 = battery.DefaultAlert1.ToString();
            DefaultAlert2 = battery.DefaultAlert2.ToString();
            CriticalBias = battery.CriticalBias.ToString();
            ChargeState = ConvertChargeState(battery.PowerState);
            PowerState = ConvertPowerState(battery.PowerState);
            PowerLineState = ConvertPowerLineState(battery.PowerState);
            CylceCount = battery.CycleCount.ToString();

            if (!double.IsNaN(battery.Temperature))
            {
                Temperature = battery.Temperature.ToString();
            }
            else
            {
                Temperature = string.Empty;
            }
        }

        private string ConvertPowerLineState(PowerStates powerState)
        {
            if ((powerState & PowerStates.PowerOnline) != 0)
            {
                return Resources.PowerStatePluggedIn;
            }

            return Resources.PowerLineStateOffline;
        }

        private string ConvertPowerState(PowerStates powerState)
        {
            if ((powerState & PowerStates.Charging) != 0)
            {
                return Resources.PowerStateCharging;
            }

            if ((powerState & PowerStates.Discharging) != 0)
            {
                return Resources.PowerStateDischarging;
            }

            return Resources.PowerStateNotCharging;
        }

        private string ConvertChargeState(PowerStates powerState)
        {
            if ((powerState & PowerStates.Critical) != 0)
            {
                return Resources.PowerStateCritical;
            }

            return Resources.PowerStateNormal;
        }

        private string ConvertChemistry(string chemistry)
        {
            switch (chemistry)
            {
                case "PbAc":
                    return Resources.ChemistryPbAc;
                case "LiP":
                    return Resources.ChemistryLiPo;
                case "Li-I":
                    return Resources.ChemistryLiIo;
                case "LION":
                    return Resources.ChemistryLiIo;
                case "NiCd":
                    return Resources.ChemistryNiCd;
                case "NiMH":
                    return Resources.ChemistryNiMH;
                case "NiZn":
                    return Resources.ChemistryNiZn;
                case "RAM":
                    return Resources.ChemistryRAM;
                default:
                    return chemistry;
            }
        }
    }
}
