using System.Diagnostics.CodeAnalysis;
using Iot.Device.CpuTemperature;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class RaspiTemperatureProvider(ILogger<RaspiTemperatureProvider> logger) : ITemperatureProvider
{
    /// <inheritdoc />
    public (double, string) GetTemperature()
    {
        using var cpuTemperature = new CpuTemperature();
        var temperatureObject = cpuTemperature.ReadTemperatures()[0];
        return !double.IsNaN(temperatureObject.Temperature.DegreesCelsius) ? (temperatureObject.Temperature.DegreesCelsius, "C") : (double.NaN, "#");
    }

    /// <inheritdoc />
    public bool IsPlatformSupported()
    {
        var isPlatformSupported = OperatingSystem.IsLinux();
        logger.LogDebug("Is platform supported: {IsPlatformSupported}", isPlatformSupported);
        return isPlatformSupported;
    }
}