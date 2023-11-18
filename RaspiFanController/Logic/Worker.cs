using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class Worker(RaspiTemperatureController raspiTemperatureController, ITaskCancellationHelper taskCancellationHelper) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        taskCancellationHelper.SetCancellationToken(stoppingToken);
        await raspiTemperatureController.StartTemperatureMeasurementAsync();
    }
}