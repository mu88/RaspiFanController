using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class DevTemperatureProvider : ITemperatureProvider
{
    private static readonly Random RandomInstance = new();

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:Arithmetic expressions should declare precedence", Justification = "Okay for me here, I'm happy")]
    public (double Temperature, string Unit) GetTemperature() => (30 + 30 * RandomInstance.NextDouble(), "C");

    /// <inheritdoc />
    public bool IsPlatformSupported() => true;
}
