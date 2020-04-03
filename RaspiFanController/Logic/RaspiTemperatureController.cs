using System.Threading;
using System.Threading.Tasks;

namespace RaspiFanController.Logic
{
    public class RaspiTemperatureController
    {
        public RaspiTemperatureController(ITemperatureProvider temperatureProvider, IFanController fanController)
        {
            TemperatureProvider = temperatureProvider;
            FanController = fanController;
            RefreshInterval = 1000;
            TemperatureThreshold = 40;
            RegulationMode = RegulationMode.Automatic;
        }

        public int RefreshInterval { get; }

        public bool IsFanRunning => FanController.IsFanRunning;

        public double CurrentTemperature { get; private set; }

        public string Unit { get; private set; }

        public RegulationMode RegulationMode { get; private set; }

        public double TemperatureThreshold { get; private set; }

        private ITemperatureProvider TemperatureProvider { get; }

        private IFanController FanController { get; }

        public void SetAutomaticTemperatureRegulation()
        {
            RegulationMode = RegulationMode.Automatic;
        }

        public void SetManualTemperatureRegulation(bool fanIsRunning)
        {
            RegulationMode = RegulationMode.Manual;

            if (fanIsRunning)
            {
                FanController.TurnFanOn();
            }
            else
            {
                FanController.TurnFanOff();
            }
        }

        public void SetTemperatureThreshold(double thresholdTemperature)
        {
            TemperatureThreshold = thresholdTemperature;
        }

        public async Task StartTemperatureMeasurementAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                (CurrentTemperature, Unit) = TemperatureProvider.GetTemperature();

                if (RegulationMode == RegulationMode.Automatic)
                {
                    if (CurrentTemperature >= TemperatureThreshold)
                    {
                        FanController.TurnFanOn();
                    }
                    else
                    {
                        FanController.TurnFanOff();
                    }
                }

                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }
    }
}