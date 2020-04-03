using System;
using System.Threading;
using System.Threading.Tasks;

namespace RaspiFanController.Logic
{
    public class RaspiTemperatureController
    {
        public RaspiTemperatureController(ITemperatureProvider temperatureProvider)
        {
            TemperatureProvider = temperatureProvider;
            RefreshInterval = 1000;
        }

        public int RefreshInterval { get; }

        public double CurrentTemperature { get; private set; }

        private ITemperatureProvider TemperatureProvider { get; }

        public void StartTemperatureRegulation()
        {
            throw new NotImplementedException();
        }

        public void StopTemperatureRegulation()
        {
            throw new NotImplementedException();
        }

        public async Task StartTemperatureMeasurementAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CurrentTemperature = TemperatureProvider.GetTemperature();

                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }
    }
}