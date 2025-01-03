namespace BatteryMonitor
{
    internal interface ISystemPowerViewModel
    {
        string PowerState { get; }

        string ChargeState { get; }

        string PowerLineStatus { get; }

        string RemainingTime { get; }

        string Capacity { get; }
    }
}
