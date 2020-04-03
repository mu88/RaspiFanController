namespace RaspiFanController.Logic
{
    public class RaspiFanController : IFanController
    {
        /// <inheritdoc />
        public bool IsFanRunning { get; }

        /// <inheritdoc />
        public void TurnFanOn()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void TurnFanOff()
        {
            throw new System.NotImplementedException();
        }
    }
}