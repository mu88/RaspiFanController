using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Unit.Logic;

[Category("Unit")]
public sealed class AppSettingsTests
{
    [Test]
    public void Validate_WhenLowerThresholdIsLessThanUpper_ReturnsNoErrors()
    {
        // Arrange
        var testee = new AppSettings { LowerTemperatureThreshold = 30, UpperTemperatureThreshold = 40 };

        // Act
        var errors = testee.Validate(new ValidationContext(testee));

        // Assert
        errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WhenLowerThresholdIsGreaterThanUpper_ReturnsValidationError()
    {
        // Arrange
        var testee = new AppSettings { LowerTemperatureThreshold = 50, UpperTemperatureThreshold = 30 };

        // Act
        var errors = testee.Validate(new ValidationContext(testee));

        // Assert
        errors.Should().ContainSingle()
            .Which.MemberNames.Should()
            .Contain(nameof(AppSettings.LowerTemperatureThreshold))
            .And.Contain(nameof(AppSettings.UpperTemperatureThreshold));
    }

    [Test]
    public void Validate_WhenLowerThresholdEqualsUpper_ReturnsValidationError()
    {
        // Arrange
        var testee = new AppSettings { LowerTemperatureThreshold = 40, UpperTemperatureThreshold = 40 };

        // Act
        var errors = testee.Validate(new ValidationContext(testee));

        // Assert
        errors.Should().ContainSingle();
    }
}
