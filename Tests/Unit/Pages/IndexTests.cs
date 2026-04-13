using System.Globalization;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using NUnit.Framework;
using RaspiFanController.Logic;
using IndexPage = RaspiFanController.Components.Pages.Index;

namespace Tests.Unit.Pages;

[Category("Unit")]
public sealed class IndexTests
{
    private BunitContext _ctx = null!;
    private IRaspiTemperatureController _controllerMock = null!;
    private FakeTimeProvider _fakeTimeProvider = null!;
    private ILogger<IndexPage> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _ctx = new BunitContext();
        _controllerMock = Substitute.For<IRaspiTemperatureController>();
        _controllerMock.RefreshMilliseconds.Returns(60_000);
        _controllerMock.CurrentTemperature.Returns(42.5);
        _controllerMock.Unit.Returns("C");
        _controllerMock.UpperTemperatureThreshold.Returns(50);
        _controllerMock.LowerTemperatureThreshold.Returns(30);
        _controllerMock.RegulationMode.Returns(RegulationMode.Automatic);
        _controllerMock.IsPlatformSupported.Returns(true);
        _controllerMock.IsFanRunning.Returns(false);

        _fakeTimeProvider = new FakeTimeProvider();
        _loggerMock = Substitute.For<ILogger<IndexPage>>();

        _ctx.Services.AddSingleton(_controllerMock);
        _ctx.Services.AddSingleton<TimeProvider>(_fakeTimeProvider);
        _ctx.Services.AddLogging();
        _ctx.Services.AddSingleton(_loggerMock);
    }

    [TearDown]
    public void TearDown() => _ctx.Dispose();

    [Test]
    public void OnInitialized_WhenAutomaticMode_FanControlsAreDisabled()
    {
        // Arrange
        _controllerMock.RegulationMode.Returns(RegulationMode.Automatic);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.Find("input[name='fanIsRunning'][value='True']").HasAttribute("disabled").Should().BeTrue();
    }

    [Test]
    public void OnInitialized_WhenManualMode_FanControlsAreEnabled()
    {
        // Arrange
        _controllerMock.RegulationMode.Returns(RegulationMode.Manual);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.Find("input[name='fanIsRunning'][value='True']").HasAttribute("disabled").Should().BeFalse();
    }

    [Test]
    public void OnInitialized_PlatformNotSupported_ShowsUnsupportedAlert()
    {
        // Arrange
        _controllerMock.IsPlatformSupported.Returns(false);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.Find(".alert-danger").TextContent.Should().Contain("not supported");
    }

    [Test]
    public void ModeChanged_ToAutomatic_CallsSetAutomaticTemperatureRegulation()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("input[name='mode'][value='Automatic']").Change("Automatic");

        // Assert
        _controllerMock.Received(1).SetAutomaticTemperatureRegulation();
    }

    [Test]
    public void ModeChanged_ToManual_CallsSetManualTemperatureRegulation()
    {
        // Arrange
        _controllerMock.IsFanRunning.Returns(false);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("input[name='mode'][value='Manual']").Change("Manual");

        // Assert
        _controllerMock.Received(1).SetManualTemperatureRegulation(false);
    }

    [Test]
    public void UpperTemperatureThresholdChanged_WithValidValue_CallsTrySetUpperTemperatureThreshold()
    {
        // Arrange
        _controllerMock.TrySetUpperTemperatureThreshold(55).Returns(true);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#upperThreshold").Change("55");

        // Assert
        _controllerMock.Received(1).TrySetUpperTemperatureThreshold(55);
    }

    [Test]
    public void UpperTemperatureThresholdChanged_WhenTrySetFails_ShowsErrorMessage()
    {
        // Arrange
        _controllerMock.TrySetUpperTemperatureThreshold(Arg.Any<int>()).Returns(false);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#upperThreshold").Change("25");

        // Assert
        cut.Find(".alert-danger").TextContent.Should().Contain("30");
    }

    [Test]
    public void UpperTemperatureThresholdChanged_WithInvalidInput_ShowsErrorMessage()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#upperThreshold").Change("not-a-number");

        // Assert
        cut.Find(".alert-danger").TextContent.Should().Contain("valid number");
    }

    [Test]
    public void LowerTemperatureThresholdChanged_WithValidValue_CallsTrySetLowerTemperatureThreshold()
    {
        // Arrange
        _controllerMock.TrySetLowerTemperatureThreshold(25).Returns(true);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#lowerThreshold").Change("25");

        // Assert
        _controllerMock.Received(1).TrySetLowerTemperatureThreshold(25);
    }

    [Test]
    public void LowerTemperatureThresholdChanged_WhenTrySetFails_ShowsErrorMessage()
    {
        // Arrange
        _controllerMock.TrySetLowerTemperatureThreshold(Arg.Any<int>()).Returns(false);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#lowerThreshold").Change("55");

        // Assert
        cut.Find(".alert-danger").TextContent.Should().Contain("50");
    }

    [Test]
    public void LowerTemperatureThresholdChanged_WithInvalidInput_ShowsErrorMessage()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("#lowerThreshold").Change("not-a-number");

        // Assert
        cut.Find(".alert-danger").TextContent.Should().Contain("valid number");
    }

    [Test]
    public void FanIsRunningChanged_WithTrueValue_CallsSetManualTemperatureRegulationTrue()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("input[name='fanIsRunning'][value='True']").Change("True");

        // Assert
        _controllerMock.Received(1).SetManualTemperatureRegulation(true);
    }

    [Test]
    public void FanIsRunningChanged_WithFalseValue_CallsSetManualTemperatureRegulationFalse()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("input[name='fanIsRunning'][value='False']").Change("False");

        // Assert
        _controllerMock.Received(1).SetManualTemperatureRegulation(false);
    }

    [Test]
    public void FanIsRunningChanged_WithInvalidValue_DefaultsToFanOn()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find("input[name='fanIsRunning'][value='True']").Change("invalid-bool");

        // Assert
        _controllerMock.Received(1).SetManualTemperatureRegulation(true);
    }

    [Test]
    public void OnTimerElapsed_WhenTimerFires_ReloadsDataAndReRenders()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();
        var initialRenderCount = cut.RenderCount;

        // Act
        _fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(60_000));

        // Assert
        cut.WaitForAssertion(() => cut.RenderCount.Should().BeGreaterThan(initialRenderCount));
    }

    [Test]
    public void OnTimerElapsed_WhenLoadDataThrows_LogsErrorWithoutPropagating()
    {
        // Arrange
        _loggerMock.IsEnabled(LogLevel.Error).Returns(true);
        var cut = _ctx.Render<IndexPage>();
        var exception = new InvalidOperationException("sensor failure");
        _controllerMock.CurrentTemperature.Returns(_ => throw exception);

        // Act
        _fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(60_000));

        // Assert
        cut.WaitForAssertion(() =>
            _loggerMock.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()));
    }
}
