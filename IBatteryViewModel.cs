namespace BatteryMonitor
{
    internal interface IBatteryViewModel
    {
        string Index { get; }

        string DeviceName { get; }

        string Manufacture { get; }

        string Chemistry { get; }

        string ManufactureDate { get; }

        string DesignedCapacity { get; }

        string CurrentCapacity { get; }

        string CurrentCapacityPercent { get; }

        string FullChargeCapacity { get; }

        string BatteryHealth { get; }

        string Voltage { get; }

        string EstimatedTime { get; }

        string Rate { get; }

        string DefaultAlert1 { get; }

        string DefaultAlert2 { get; }

        string CriticalBias { get; }

        string ChargeState { get; }

        string PowerState { get; }

        string PowerLineState { get; }

        string CylceCount { get; }

        string Temperature { get; }
    }
}
