using System.Device.Gpio;

namespace RaspiFanController.Logic
{
    public class RaspiFanController : IFanController
    {
        public RaspiFanController()
        {
            GpioController = new GpioController();
            GpioPin = 17;
        }

        /// <inheritdoc />
        public bool IsFanRunning => GpioController.Read(GpioPin) == PinValue.High;

        private GpioController GpioController { get; }

        private int GpioPin { get; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            GpioController.Write(GpioPin, PinValue.High);
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            GpioController.Write(GpioPin, PinValue.Low);
        }
    }
}