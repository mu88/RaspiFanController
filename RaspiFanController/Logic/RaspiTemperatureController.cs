using System.Diagnostics;
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
            MinimumSleepTime = 60;
            RefreshInterval = 1000;
            TemperatureThreshold = 40;
            RegulationMode = RegulationMode.Automatic;
            Stopwatch = new Stopwatch();
        }

        public bool IsPlatformSupported => TemperatureProvider.IsPlatformSupported();

        public int RefreshInterval { get; }

        public bool IsFanRunning => FanController.IsFanRunning;

        public int MinimumSleepTime { get; private set; }

        public double CurrentTemperature { get; private set; }

        public string Unit { get; private set; }

        public RegulationMode RegulationMode { get; private set; }

        public int TemperatureThreshold { get; private set; }

        private Stopwatch Stopwatch { get; }

        private ITemperatureProvider TemperatureProvider { get; }

        private IFanController FanController { get; }

        public void SetAutomaticTemperatureRegulation()
        {
            RegulationMode = RegulationMode.Automatic;
        }

        public void SetManualTemperatureRegulation(bool fanShouldRun)
        {
            RegulationMode = RegulationMode.Manual;

            if (fanShouldRun)
            {
                if (!IsFanRunning)
                {
                    FanController.TurnFanOn();
                }
            }
            else
            {
                if (IsFanRunning)
                {
                    FanController.TurnFanOff();
                }
            }
        }

        public void SetTemperatureThreshold(int thresholdTemperature)
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
                        if (!FanController.IsFanRunning && SleepTimeReached())
                        {
                            FanController.TurnFanOn();
                        }
                    }
                    else
                    {
                        if (IsFanRunning && SleepTimeReached())
                        {
                            FanController.TurnFanOff();
                        }
                    }
                }

                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }

        public void SetSleepTime(int sleepTime)
        {
            MinimumSleepTime = sleepTime;
        }

        private bool SleepTimeReached()
        {
            if (!Stopwatch.IsRunning)
            {
                Stopwatch.Restart();
                return false;
            }

            if (Stopwatch.Elapsed.TotalSeconds > MinimumSleepTime)
            {
                Stopwatch.Restart();
                return true;
            }

            return false;
        }
    }
}