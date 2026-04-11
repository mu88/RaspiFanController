using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
internal class Worker(IRaspiTemperatureController raspiTemperatureController) : BackgroundService
{
    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => raspiTemperatureController.StartTemperatureMeasurementAsync(stoppingToken);
}
