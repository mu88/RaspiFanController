using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

public class RaspiTemperatureController(
    ITemperatureProvider temperatureProvider,
    IFanController fanController,
    ITaskCancellationHelper taskCancellationHelper,
    ITaskHelper taskHelper,
    ILogger<RaspiTemperatureController> logger,
    IOptionsMonitor<AppSettings> settings)
{
    public bool IsPlatformSupported => temperatureProvider.IsPlatformSupported();

    public int RefreshMilliseconds { get; } = settings.CurrentValue.RefreshMilliseconds;

    public bool IsFanRunning => fanController.IsFanRunning;

    public TimeSpan Uptime => DateTime.Now - StartTime;

    public int LowerTemperatureThreshold { get; private set; } = settings.CurrentValue.LowerTemperatureThreshold;

    public double CurrentTemperature { get; private set; }

    // Stryker disable all : don't mutate default initialization
    public string Unit { get; private set; } = string.Empty;

    // Stryker restore all
    public RegulationMode RegulationMode { get; private set; } = RegulationMode.Automatic;

    public int UpperTemperatureThreshold { get; private set; } = settings.CurrentValue.UpperTemperatureThreshold;

    private DateTime StartTime { get; } = DateTime.Now;

    public void SetAutomaticTemperatureRegulation()
    {
        RegulationMode = RegulationMode.Automatic;
        logger.LogInformation("Set automatic mode");
    }

    public void SetManualTemperatureRegulation(bool fanShouldRun)
    {
        RegulationMode = RegulationMode.Manual;

        if (fanShouldRun)
        {
            if (!IsFanRunning)
            {
                fanController.TurnFanOn();
                logger.LogDebug("Set manual mode and turned on");
            }
        }
        else
        {
            if (IsFanRunning)
            {
                fanController.TurnFanOff();
                logger.LogDebug("Set manual mode and turned on");
            }
        }
    }

    public bool TrySetUpperTemperatureThreshold(int upperTemperatureThreshold)
    {
        if (upperTemperatureThreshold <= LowerTemperatureThreshold)
        {
            return false;
        }

        UpperTemperatureThreshold = upperTemperatureThreshold;
        logger.LogInformation("Set upper threshold");

        return true;
    }

    public async Task StartTemperatureMeasurementAsync()
    {
        while (!taskCancellationHelper.IsCancellationRequested)
        {
            (CurrentTemperature, Unit) = temperatureProvider.GetTemperature();
            logger.LogDebug("Current: {CurrentTemperature}°{Unit}", CurrentTemperature, Unit);

            if (RegulationMode == RegulationMode.Automatic)
            {
                if (CurrentTemperature >= UpperTemperatureThreshold)
                {
                    if (!fanController.IsFanRunning)
                    {
                        fanController.TurnFanOn();
                        logger.LogDebug("Turned fan on in automatic mode");
                    }
                }
                else if (CurrentTemperature < LowerTemperatureThreshold && IsFanRunning)
                {
                    fanController.TurnFanOff();
                    logger.LogDebug("Turned fan off in automatic mode");
                }
            }

            await taskHelper.DelayAsync(RefreshMilliseconds, taskCancellationHelper.CancellationToken);
        }
    }

    public bool TrySetLowerTemperatureThreshold(int lowerTemperatureThreshold)
    {
        if (lowerTemperatureThreshold >= UpperTemperatureThreshold)
        {
            return false;
        }

        LowerTemperatureThreshold = lowerTemperatureThreshold;
        logger.LogInformation("Set lower threshold");

        return true;
    }
}