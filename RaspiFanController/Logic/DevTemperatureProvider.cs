using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class DevTemperatureProvider : ITemperatureProvider
{
    /// <inheritdoc />
    public (double, string) GetTemperature()
    {
        return (30 + 30 * new Random().NextDouble(), "C");
    }

    /// <inheritdoc />
    public bool IsPlatformSupported()
    {
        return true;
    }
}