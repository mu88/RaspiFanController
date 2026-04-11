using System.ComponentModel.DataAnnotations;

namespace RaspiFanController.Logic;

public class AppSettings : IValidatableObject
{
    public const string SectionName = "AppSettings";

    [Range(100, 60_000)]
    public int RefreshMilliseconds { get; set; }

    [Range(1, 100)]
    public int UpperTemperatureThreshold { get; set; }

    [Range(1, 100)]
    public int LowerTemperatureThreshold { get; set; }

    [Range(1, 40)]
    public int GpioPin { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LowerTemperatureThreshold >= UpperTemperatureThreshold)
        {
            yield return new ValidationResult(
                $"{nameof(LowerTemperatureThreshold)} must be less than {nameof(UpperTemperatureThreshold)}",
                [nameof(LowerTemperatureThreshold), nameof(UpperTemperatureThreshold)]);
        }
    }
}
