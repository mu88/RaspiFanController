using System.Threading.Tasks;

namespace RaspiFanController.Logic
{
    public class RaspiTemperatureController
    {
        public RaspiTemperatureController(ITemperatureProvider temperatureProvider,
                                          IFanController fanController,
                                          ITaskCancellationHelper taskCancellationHelper,
                                          ITaskHelper taskHelper)
        {
            TemperatureProvider = temperatureProvider;
            FanController = fanController;
            RefreshInterval = 1000;
            UpperTemperatureThreshold = 40;
            LowerTemperatureThreshold = 30;
            RegulationMode = RegulationMode.Automatic;
            TaskCancellationHelper = taskCancellationHelper;
            TaskHelper = taskHelper;
        }

        public bool IsPlatformSupported => TemperatureProvider.IsPlatformSupported();

        public int RefreshInterval { get; }

        public bool IsFanRunning => FanController.IsFanRunning;

        public int LowerTemperatureThreshold { get; private set; }

        public double CurrentTemperature { get; private set; }

        public string Unit { get; private set; }

        public RegulationMode RegulationMode { get; private set; }

        public int UpperTemperatureThreshold { get; private set; }

        private ITemperatureProvider TemperatureProvider { get; }

        private IFanController FanController { get; }

        private ITaskCancellationHelper TaskCancellationHelper { get; }

        private ITaskHelper TaskHelper { get; }

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

        public bool TrySetUpperTemperatureThreshold(int upperTemperatureThreshold)
        {
            if (upperTemperatureThreshold <= LowerTemperatureThreshold)
            {
                return false;
            }

            UpperTemperatureThreshold = upperTemperatureThreshold;

            return true;
        }

        public async Task StartTemperatureMeasurementAsync()
        {
            while (!TaskCancellationHelper.IsCancellationRequested)
            {
                (CurrentTemperature, Unit) = TemperatureProvider.GetTemperature();

                if (RegulationMode == RegulationMode.Automatic)
                {
                    if (CurrentTemperature >= UpperTemperatureThreshold)
                    {
                        if (!FanController.IsFanRunning)
                        {
                            FanController.TurnFanOn();
                        }
                    }
                    else if (CurrentTemperature < LowerTemperatureThreshold)
                    {
                        if (IsFanRunning)
                        {
                            FanController.TurnFanOff();
                        }
                    }
                }

                await TaskHelper.Delay(RefreshInterval, TaskCancellationHelper.CancellationToken);
            }
        }

        public bool TrySetLowerTemperatureThreshold(int lowerTemperatureThreshold)
        {
            if (lowerTemperatureThreshold >= UpperTemperatureThreshold)
            {
                return false;
            }

            LowerTemperatureThreshold = lowerTemperatureThreshold;

            return true;
        }
    }
}