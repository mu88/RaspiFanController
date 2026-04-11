using System.Diagnostics.CodeAnalysis;
using Iot.Device.CpuTemperature;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public partial class RaspiTemperatureProvider(ILogger<RaspiTemperatureProvider> logger) : ITemperatureProvider
{
    /// <inheritdoc />
    public (double Temperature, string Unit) GetTemperature()
    {
        using var cpuTemperature = new CpuTemperature();
        var temperatures = cpuTemperature.ReadTemperatures();
        if (temperatures.Count == 0)
        {
            return (double.NaN, "#");
        }

        var temperatureObject = temperatures[0];
        return !double.IsNaN(temperatureObject.Temperature.DegreesCelsius)
            ? (temperatureObject.Temperature.DegreesCelsius, "C")
            : (double.NaN, "#");
    }

    /// <inheritdoc />
    public bool IsPlatformSupported()
    {
        var isPlatformSupported = OperatingSystem.IsLinux();
        LogIsPlatformSupported(isPlatformSupported);
        return isPlatformSupported;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Is platform supported: {IsPlatformSupported}")]
    private partial void LogIsPlatformSupported(bool isPlatformSupported);
}
