using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

public class RaspiTemperatureController(ITemperatureProvider temperatureProvider,
                                        IFanController fanController,
                                        ITaskCancellationHelper taskCancellationHelper,
                                        ITaskHelper taskHelper,
                                        ILogger<RaspiTemperatureController> logger,
                                        IOptionsMonitor<AppSettings> settings)
{
    public bool IsPlatformSupported => TemperatureProvider.IsPlatformSupported();

    public int RefreshMilliseconds { get; } = settings.CurrentValue.RefreshMilliseconds;

    public bool IsFanRunning => FanController.IsFanRunning;

    public TimeSpan Uptime => DateTime.Now - StartTime;

    public int LowerTemperatureThreshold { get; private set; } = settings.CurrentValue.LowerTemperatureThreshold;

    public double CurrentTemperature { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public RegulationMode RegulationMode { get; private set; } = RegulationMode.Automatic;

    public int UpperTemperatureThreshold { get; private set; } = settings.CurrentValue.UpperTemperatureThreshold;

    private ITemperatureProvider TemperatureProvider { get; } = temperatureProvider;

    private IFanController FanController { get; } = fanController;

    private ITaskCancellationHelper TaskCancellationHelper { get; } = taskCancellationHelper;

    private ITaskHelper TaskHelper { get; } = taskHelper;

    private ILogger<RaspiTemperatureController> Logger { get; } = logger;

    private DateTime StartTime { get; } = DateTime.Now;

    public void SetAutomaticTemperatureRegulation()
    {
        RegulationMode = RegulationMode.Automatic;
        Logger.LogInformation("Set automatic mode");
    }

    public void SetManualTemperatureRegulation(bool fanShouldRun)
    {
        RegulationMode = RegulationMode.Manual;

        if (fanShouldRun)
        {
            if (!IsFanRunning)
            {
                FanController.TurnFanOn();
                Logger.LogDebug("Set manual mode and turned on");
            }
        }
        else
        {
            if (IsFanRunning)
            {
                FanController.TurnFanOff();
                Logger.LogDebug("Set manual mode and turned on");
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
        Logger.LogInformation("Set upper threshold");

        return true;
    }

    public async Task StartTemperatureMeasurementAsync()
    {
        while (!TaskCancellationHelper.IsCancellationRequested)
        {
            (CurrentTemperature, Unit) = TemperatureProvider.GetTemperature();
            Logger.LogDebug("Current: {CurrentTemperature}°{Unit}", CurrentTemperature, Unit);

            if (RegulationMode == RegulationMode.Automatic)
            {
                if (CurrentTemperature >= UpperTemperatureThreshold)
                {
                    if (!FanController.IsFanRunning)
                    {
                        FanController.TurnFanOn();
                        Logger.LogDebug("Turned fan on in automatic mode");
                    }
                }
                else if (CurrentTemperature < LowerTemperatureThreshold && IsFanRunning)
                {
                    FanController.TurnFanOff();
                    Logger.LogDebug("Turned fan off in automatic mode");
                }
            }

            await TaskHelper.Delay(RefreshMilliseconds, TaskCancellationHelper.CancellationToken);
        }
    }

    public bool TrySetLowerTemperatureThreshold(int lowerTemperatureThreshold)
    {
        if (lowerTemperatureThreshold >= UpperTemperatureThreshold)
        {
            return false;
        }

        LowerTemperatureThreshold = lowerTemperatureThreshold;
        Logger.LogInformation("Set lower threshold");

        return true;
    }
}