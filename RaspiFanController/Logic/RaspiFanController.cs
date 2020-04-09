using System.Device.Gpio;

namespace RaspiFanController.Logic
{
    public class RaspiFanController : IFanController
    {
        public RaspiFanController()
        {
            GpioPin = 17;

            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Input);
            IsFanRunning = gpioController.Read(GpioPin) == PinValue.High;
        }

        /// <inheritdoc />
        public bool IsFanRunning { get; private set; }

        private int GpioPin { get; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.High);
            IsFanRunning = true;
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.Low);
            IsFanRunning = false;
        }
    }
}