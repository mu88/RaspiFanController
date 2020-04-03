namespace RaspiFanController.Logic
{
    public class DevFanController : IFanController
    {
        /// <inheritdoc />
        public bool IsFanRunning { get; private set; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            IsFanRunning = true;
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            IsFanRunning = false;
        }
    }
}