using System;
using System.Threading.Tasks;

namespace Logic
{
    public class TemperatureController
    {
        public TemperatureController(ITemperatureProvider temperatureProvider, int refreshInterval = 1000)
        {
            TemperatureProvider = temperatureProvider;
            RefreshInterval = refreshInterval;
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

        public void StartTemperatureMeasurement()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    CurrentTemperature = TemperatureProvider.GetTemperature();

                    await Task.Delay(RefreshInterval);
                }
            });
        }
    }
}