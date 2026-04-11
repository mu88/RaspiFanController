using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Unit.Logic;

[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class RaspiTemperatureControllerTests
{
    private readonly ITemperatureProvider _temperatureProviderMock = Substitute.For<ITemperatureProvider>();
    private readonly IFanController _fanControllerMock = Substitute.For<IFanController>();
    private readonly IOptions<AppSettings> _optionsMock = Substitute.For<IOptions<AppSettings>>();
    private readonly FakeTimeProvider _fakeTimeProvider = new();

    [TestCase(61, 40)]
    [TestCase(61, 61)]
    public async Task TurnFanOnInAutomaticMode(int currentTemperature, int upperTemperatureThreshold)
    {
        // Arrange
        _temperatureProviderMock.GetTemperature().Returns((currentTemperature, "C"));
        _fanControllerMock.IsFanRunning.Returns(false);
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = upperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();
        using var cts = new CancellationTokenSource();

        // Act
        var task = testee.StartTemperatureMeasurementAsync(cts.Token);

        // Verify that the delay is actually awaited
        task.IsCompleted.Should().BeFalse();
        await cts.CancelAsync();
        await task;

        // Assert
        _fanControllerMock.Received(1).TurnFanOn();
    }

    [TestCase(29, 30, true, true)]
    [TestCase(30, 30, true, false)]
    [TestCase(29, 30, false, false)]
    [TestCase(31, 30, true, false)]
    [TestCase(29, 30, false, false)]
    public async Task TurnFanOffInAutomaticMode(
        int currentTemperature,
        int lowerTemperatureThreshold,
        bool fanIsRunning,
        bool expectedFanToBeTurnedOff)
    {
        // Arrange
        _temperatureProviderMock.GetTemperature().Returns((currentTemperature, "C"));
        _fanControllerMock.IsFanRunning.Returns(fanIsRunning);
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = lowerTemperatureThreshold });
        var testee = CreateTestee();
        using var cts = new CancellationTokenSource();

        // Act
        var task = testee.StartTemperatureMeasurementAsync(cts.Token);

        // Verify that the delay is actually awaited
        task.IsCompleted.Should().BeFalse();
        await cts.CancelAsync();
        await task;

        // Assert
        _fanControllerMock.Received(expectedFanToBeTurnedOff ? 1 : 0).TurnFanOff();
    }

    [Test]
    public async Task DeriveUnitFromMeasuredTemperature()
    {
        // Arrange
        _temperatureProviderMock.GetTemperature().Returns((39, "C"));
        _fanControllerMock.IsFanRunning.Returns(true);
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();
        using var cts = new CancellationTokenSource();

        // Act
        var task = testee.StartTemperatureMeasurementAsync(cts.Token);
        task.IsCompleted.Should().BeFalse();
        await cts.CancelAsync();
        await task;

        // Assert
        testee.Unit.Should().Be("C");
    }

    [Test]
    public void IsPlatformSupported()
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // Act
        // ReSharper disable once UnusedVariable - Reviewed: the result is not necessary, only the call is important
        var result = testee.IsPlatformSupported;

        // Assert
        _temperatureProviderMock.Received(1).IsPlatformSupported();
    }

    [Test]
    public void TrySetUpperTemperatureThreshold()
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // Act
        var result = testee.TrySetUpperTemperatureThreshold(35);

        // Assert
        result.Should().BeTrue();
        testee.UpperTemperatureThreshold.Should().Be(35);
    }

    [TestCase(10, 30)]
    [TestCase(30, 30)]
    public void TrySetUpperTemperatureThresholdFailsForTooLowTemperature(int newUpperTemperatureThreshold, int currentLowerTemperatureThreshold)
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = currentLowerTemperatureThreshold });
        var testee = CreateTestee();

        // Act
        var result = testee.TrySetUpperTemperatureThreshold(newUpperTemperatureThreshold);

        // Assert
        result.Should().BeFalse();
        testee.UpperTemperatureThreshold.Should().Be(40);
    }

    [Test]
    public async Task GetUptime()
    {
        // Arrange
        _temperatureProviderMock.GetTemperature().Returns((39, "C"));
        _fanControllerMock.IsFanRunning.Returns(true);
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();
        using var cts = new CancellationTokenSource();

        // Act
        var task = testee.StartTemperatureMeasurementAsync(cts.Token);
        task.IsCompleted.Should().BeFalse();
        await cts.CancelAsync();
        await task;
        _fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(1));

        // Assert
        testee.Uptime.Should().BePositive();
    }

    [Test]
    public void TrySetLowerTemperatureThreshold()
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // Act
        var result = testee.TrySetLowerTemperatureThreshold(35);

        // Assert
        result.Should().BeTrue();
        testee.LowerTemperatureThreshold.Should().Be(35);
    }

    [TestCase(50, 40)]
    [TestCase(50, 50)]
    public void TrySetLowerTemperatureThresholdFailsForTooHighTemperature(int newLowerTemperatureThreshold, int currentUpperTemperatureThreshold)
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = currentUpperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // Act
        var result = testee.TrySetLowerTemperatureThreshold(newLowerTemperatureThreshold);

        // Assert
        result.Should().BeFalse();
        testee.LowerTemperatureThreshold.Should().Be(30);
    }

    [Test]
    public void SetAutomaticTemperatureRegulation()
    {
        // Arrange
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();
        testee.SetManualTemperatureRegulation(false);

        // Act
        testee.SetAutomaticTemperatureRegulation();

        // Assert
        testee.RegulationMode.Should().Be(RegulationMode.Automatic);
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void SetManualTemperatureRegulation(bool fanShouldRun, bool isFanRunning)
    {
        // Arrange
        _fanControllerMock.IsFanRunning.Returns(isFanRunning);
        _optionsMock.Value.Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // Act
        testee.SetManualTemperatureRegulation(fanShouldRun);

        // Assert
        testee.RegulationMode.Should().Be(RegulationMode.Manual);
        if (fanShouldRun)
        {
            if (isFanRunning)
            {
                _fanControllerMock.DidNotReceive().TurnFanOn();
            }
            else
            {
                _fanControllerMock.Received(1).TurnFanOn();
            }
        }
        else
        {
            if (isFanRunning)
            {
                _fanControllerMock.Received(1).TurnFanOff();
            }
            else
            {
                _fanControllerMock.DidNotReceive().TurnFanOff();
            }
        }
    }

    private RaspiTemperatureController CreateTestee()
        => new(_temperatureProviderMock,
            _fanControllerMock,
            _fakeTimeProvider,
            Substitute.For<ILogger<RaspiTemperatureController>>(),
            _optionsMock);
}