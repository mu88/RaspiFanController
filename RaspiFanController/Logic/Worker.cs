using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace RaspiFanController.Logic
{
    public class Worker : BackgroundService
    {
        /// <inheritdoc />
        public Worker(RaspiTemperatureController raspiTemperatureController)
        {
            RaspiTemperatureController = raspiTemperatureController;
        }

        private RaspiTemperatureController RaspiTemperatureController { get; }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RaspiTemperatureController.StartTemperatureMeasurementAsync(stoppingToken);
        }
    }
}