using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class AppSettings
{
    public const string SectionName = "AppSettings";

    public int RefreshMilliseconds { get; set; }

    public int UpperTemperatureThreshold { get; set; }

    public int LowerTemperatureThreshold { get; set; }

    public int GpioPin { get; set; }

    public string AppPathBase { get; set; }
}