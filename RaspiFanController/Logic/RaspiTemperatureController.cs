using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

internal partial class RaspiTemperatureController(
    ITemperatureProvider temperatureProvider,
    IFanController fanController,
    TimeProvider timeProvider,
    ILogger<RaspiTemperatureController> logger,
    IOptions<AppSettings> settings) : IRaspiTemperatureController
{
    private double _currentTemperature;

    // Stryker disable all : don't mutate default initialization
    private string _unit = string.Empty;

    // Stryker restore all
    public bool IsPlatformSupported => temperatureProvider.IsPlatformSupported();

    public int RefreshMilliseconds { get; } = settings.Value.RefreshMilliseconds;

    public bool IsFanRunning => fanController.IsFanRunning;

    public TimeSpan Uptime => timeProvider.GetUtcNow() - StartTime;

    public int LowerTemperatureThreshold { get; private set; } = settings.Value.LowerTemperatureThreshold;

    public double CurrentTemperature
    {
        get => Volatile.Read(ref _currentTemperature);
        private set => Volatile.Write(ref _currentTemperature, value);
    }

    public string Unit
    {
        get => Volatile.Read(ref _unit);
        private set => Volatile.Write(ref _unit, value);
    }

    public RegulationMode RegulationMode { get; private set; } = RegulationMode.Automatic;

    public int UpperTemperatureThreshold { get; private set; } = settings.Value.UpperTemperatureThreshold;

    private DateTimeOffset StartTime { get; } = timeProvider.GetUtcNow();

    public void SetAutomaticTemperatureRegulation()
    {
        RegulationMode = RegulationMode.Automatic;
        LogSetAutomaticMode();
    }

    public void SetManualTemperatureRegulation(bool fanShouldRun)
    {
        RegulationMode = RegulationMode.Manual;

        if (fanShouldRun && !IsFanRunning)
        {
            fanController.TurnFanOn();
            LogSetManualModeTurnedOn();
        }
        else if (!fanShouldRun && IsFanRunning)
        {
            fanController.TurnFanOff();
            LogSetManualModeTurnedOff();
        }
        else
        {
            LogSetManualModeUnchanged();
        }
    }

    public bool TrySetUpperTemperatureThreshold(int upperTemperatureThreshold)
    {
        if (upperTemperatureThreshold <= LowerTemperatureThreshold)
        {
            return false;
        }

        UpperTemperatureThreshold = upperTemperatureThreshold;
        LogSetUpperThreshold();

        return true;
    }

    public async Task StartTemperatureMeasurementAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            (CurrentTemperature, Unit) = temperatureProvider.GetTemperature();
            LogCurrentTemperature(CurrentTemperature, Unit);

            if (RegulationMode == RegulationMode.Automatic
                && CurrentTemperature >= UpperTemperatureThreshold
                && !fanController.IsFanRunning)
            {
                fanController.TurnFanOn();
                LogTurnedFanOnInAutomaticMode();
            }
            else if (RegulationMode == RegulationMode.Automatic
                     && CurrentTemperature < LowerTemperatureThreshold
                     && IsFanRunning)
            {
                fanController.TurnFanOff();
                LogTurnedFanOffInAutomaticMode();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(RefreshMilliseconds), timeProvider, cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    public bool TrySetLowerTemperatureThreshold(int lowerTemperatureThreshold)
    {
        if (lowerTemperatureThreshold >= UpperTemperatureThreshold)
        {
            return false;
        }

        LowerTemperatureThreshold = lowerTemperatureThreshold;
        LogSetLowerThreshold();

        return true;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Current: {CurrentTemperature}°{Unit}")]
    private partial void LogCurrentTemperature(double currentTemperature, string unit);

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Set automatic mode")]
    private partial void LogSetAutomaticMode();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Set manual mode and turned on")]
    private partial void LogSetManualModeTurnedOn();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Set manual mode and turned off")]
    private partial void LogSetManualModeTurnedOff();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Set manual mode, fan state unchanged")]
    private partial void LogSetManualModeUnchanged();

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Set upper threshold")]
    private partial void LogSetUpperThreshold();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Turned fan on in automatic mode")]
    private partial void LogTurnedFanOnInAutomaticMode();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Turned fan off in automatic mode")]
    private partial void LogTurnedFanOffInAutomaticMode();

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Set lower threshold")]
    private partial void LogSetLowerThreshold();
}