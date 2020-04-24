namespace RaspiFanController.Logic
{
    public class AppSettings
    {
        public int RefreshMilliseconds { get; set; }

        public int UpperTemperatureThreshold { get; set; }

        public int LowerTemperatureThreshold { get; set; }

        public int GpioPin { get; set; }

        public string AppPathBase { get; set; }
    }
}