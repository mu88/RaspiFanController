﻿using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

public class RaspiTemperatureController
{
    public RaspiTemperatureController(ITemperatureProvider temperatureProvider,
                                      IFanController fanController,
                                      ITaskCancellationHelper taskCancellationHelper,
                                      ITaskHelper taskHelper,
                                      ILogger<RaspiTemperatureController> logger,
                                      IOptionsMonitor<AppSettings> settings)
    {
        TemperatureProvider = temperatureProvider;
        FanController = fanController;
        RefreshMilliseconds = settings.CurrentValue.RefreshMilliseconds;
        UpperTemperatureThreshold = settings.CurrentValue.UpperTemperatureThreshold;
        LowerTemperatureThreshold = settings.CurrentValue.LowerTemperatureThreshold;
        RegulationMode = RegulationMode.Automatic;
        StartTime = DateTime.Now;
        TaskCancellationHelper = taskCancellationHelper;
        TaskHelper = taskHelper;
        Logger = logger;
    }

    public bool IsPlatformSupported => TemperatureProvider.IsPlatformSupported();

    public int RefreshMilliseconds { get; }

    public bool IsFanRunning => FanController.IsFanRunning;

    public TimeSpan Uptime => DateTime.Now - StartTime;

    public int LowerTemperatureThreshold { get; private set; }

    public double CurrentTemperature { get; private set; }

    public string Unit { get; private set; }

    public RegulationMode RegulationMode { get; private set; }

    public int UpperTemperatureThreshold { get; private set; }

    private ITemperatureProvider TemperatureProvider { get; }

    private IFanController FanController { get; }

    private ITaskCancellationHelper TaskCancellationHelper { get; }

    private ITaskHelper TaskHelper { get; }

    private ILogger<RaspiTemperatureController> Logger { get; }

    private DateTime StartTime { get; }

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
            Logger.LogDebug($"Current: {CurrentTemperature}°{Unit}");

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
                else if (CurrentTemperature < LowerTemperatureThreshold)
                {
                    if (IsFanRunning)
                    {
                        FanController.TurnFanOff();
                        Logger.LogDebug("Turned fan off in automatic mode");
                    }
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