using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace RaspiFanController.Logic
{
    public class RaspiFanController : IFanController
    {
        public RaspiFanController(ILogger<RaspiFanController> logger)
        {
            Logger = logger;
            GpioPin = 17;

            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Input);
            var initialValue = gpioController.Read(GpioPin) == PinValue.High;
            IsFanRunning = initialValue;

            logger.LogInformation($"Initial value: {initialValue}");
        }

        /// <inheritdoc />
        public bool IsFanRunning { get; private set; }

        private ILogger<RaspiFanController> Logger { get; }

        private int GpioPin { get; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.High);
            IsFanRunning = true;

            Logger.LogInformation("Fan turned on");
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.Low);
            IsFanRunning = false;

            Logger.LogInformation("Fan turned off");
        }
    }
}