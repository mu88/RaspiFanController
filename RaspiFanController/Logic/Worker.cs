using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class Worker : BackgroundService
{
    /// <inheritdoc />
    public Worker(RaspiTemperatureController raspiTemperatureController, ITaskCancellationHelper taskCancellationHelper)
    {
        RaspiTemperatureController = raspiTemperatureController;
        TaskCancellationHelper = taskCancellationHelper;
    }

    private RaspiTemperatureController RaspiTemperatureController { get; }

    private ITaskCancellationHelper TaskCancellationHelper { get; }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TaskCancellationHelper.SetCancellationToken(stoppingToken);
        await RaspiTemperatureController.StartTemperatureMeasurementAsync();
    }
}