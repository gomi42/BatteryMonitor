namespace BatteryMonitor
{
    internal interface ISystemPower
    {
        string PowerState { get; }

        string ChargeState { get; }

        string PowerLineStatus { get; }

        string RemainingTime { get; }

        string Capacity { get; }
    }
}
