using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class DevTemperatureProvider : ITemperatureProvider
{
    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:Arithmetic expressions should declare precedence", Justification = "Okay for me here, I'm happy")]
    public (double, string) GetTemperature() => (30 + 30 * new Random().NextDouble(), "C");

    /// <inheritdoc />
    public bool IsPlatformSupported() => true;
}