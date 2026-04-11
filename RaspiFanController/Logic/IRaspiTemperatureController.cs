namespace RaspiFanController.Logic;

internal interface IRaspiTemperatureController
{
    bool IsPlatformSupported { get; }

    int RefreshMilliseconds { get; }

    bool IsFanRunning { get; }

    TimeSpan Uptime { get; }

    int LowerTemperatureThreshold { get; }

    double CurrentTemperature { get; }

    string Unit { get; }

    RegulationMode RegulationMode { get; }

    int UpperTemperatureThreshold { get; }

    void SetAutomaticTemperatureRegulation();

    void SetManualTemperatureRegulation(bool fanShouldRun);

    bool TrySetUpperTemperatureThreshold(int upperTemperatureThreshold);

    bool TrySetLowerTemperatureThreshold(int lowerTemperatureThreshold);

    Task StartTemperatureMeasurementAsync(CancellationToken cancellationToken);
}
