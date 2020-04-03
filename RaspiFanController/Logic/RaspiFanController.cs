using System.Device.Gpio;

namespace RaspiFanController.Logic
{
    public class RaspiFanController : IFanController
    {
        public RaspiFanController()
        {
            GpioPin = 17;
        }

        /// <inheritdoc />
        public bool IsFanRunning
        {
            get
            {
                var gpioController = new GpioController();
                gpioController.OpenPin(GpioPin, PinMode.Input);
                return gpioController.Read(GpioPin) == PinValue.High;
            }
        }

        private int GpioPin { get; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.High);
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            var gpioController = new GpioController();
            gpioController.OpenPin(GpioPin, PinMode.Output);
            gpioController.Write(GpioPin, PinValue.Low);
        }
    }
}