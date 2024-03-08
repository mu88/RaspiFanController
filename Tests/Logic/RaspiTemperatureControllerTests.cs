using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Logic;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class RaspiTemperatureControllerTests
{
    private readonly ITaskCancellationHelper _taskCancellationHelperMock = Substitute.For<ITaskCancellationHelper>();
    private readonly ITemperatureProvider _temperatureProviderMock = Substitute.For<ITemperatureProvider>();
    private readonly IFanController _fanControllerMock = Substitute.For<IFanController>();
    private readonly IOptionsMonitor<AppSettings> _optionsMonitorMock = Substitute.For<IOptionsMonitor<AppSettings>>();
    private readonly ITaskHelper _taskHelperMock = Substitute.For<ITaskHelper>();

    [TestCase(61, 40)]
    [TestCase(61, 61)]
    public async Task TurnFanOnInAutomaticMode(int currentTemperature, int upperTemperatureThreshold)
    {
        _taskCancellationHelperMock.IsCancellationRequested.Returns(false, true);
        _temperatureProviderMock.GetTemperature().Returns((currentTemperature, "C"));
        _fanControllerMock.IsFanRunning.Returns(false);
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = upperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        await testee.StartTemperatureMeasurementAsync();

        _fanControllerMock.Received(1).TurnFanOn();
        await _taskHelperMock.Received(1).DelayAsync(testee.RefreshMilliseconds, Arg.Any<CancellationToken>());
    }

    [TestCase(29, 30, true, true)]
    [TestCase(29, 30, false, false)]
    [TestCase(30, 30, true, false)]
    [TestCase(31, 30, true, false)]
    [TestCase(29, 30, false, false)]
    public async Task TurnFanOffInAutomaticMode(int currentTemperature,
                                                int lowerTemperatureThreshold,
                                                bool fanIsRunning,
                                                bool expectedFanToBeTurnedOff)
    {
        _taskCancellationHelperMock.IsCancellationRequested.Returns(false, true);
        _temperatureProviderMock.GetTemperature().Returns((currentTemperature, "C"));
        _fanControllerMock.IsFanRunning.Returns(fanIsRunning);
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = lowerTemperatureThreshold });
        var testee = CreateTestee();

        await testee.StartTemperatureMeasurementAsync();

        if (expectedFanToBeTurnedOff) _fanControllerMock.Received(1).TurnFanOff();
        await _taskHelperMock.Received(1).DelayAsync(testee.RefreshMilliseconds, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task DeriveUnitFromMeasuredTemperature()
    {
        _taskCancellationHelperMock.IsCancellationRequested.Returns(false, true);
        _temperatureProviderMock.GetTemperature().Returns((39, "C"));
        _fanControllerMock.IsFanRunning.Returns(true);
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        await testee.StartTemperatureMeasurementAsync();

        testee.Unit.Should().Be("C");
    }

    [Test]
    public void IsPlatformSupported()
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        // ReSharper disable once UnusedVariable - Reviewed: the result is not necessary, only the call is important
        var result = testee.IsPlatformSupported;

        _temperatureProviderMock.Received(1).IsPlatformSupported();
    }

    [Test]
    public void TrySetUpperTemperatureThreshold()
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        var result = testee.TrySetUpperTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.UpperTemperatureThreshold.Should().Be(35);
    }

    [TestCase(10, 30)]
    [TestCase(30, 30)]
    public void TrySetUpperTemperatureThresholdFailsForTooLowTemperature(int newUpperTemperatureThreshold, int currentLowerTemperatureThreshold)
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = currentLowerTemperatureThreshold });
        var testee = CreateTestee();

        var result = testee.TrySetUpperTemperatureThreshold(newUpperTemperatureThreshold);

        result.Should().BeFalse();
        testee.UpperTemperatureThreshold.Should().Be(40);
    }

    [Test]
    public async Task GetUptime()
    {
        _taskCancellationHelperMock.IsCancellationRequested.Returns(false, true);
        _temperatureProviderMock.GetTemperature().Returns((39, "C"));
        _fanControllerMock.IsFanRunning.Returns(true);
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        await testee.StartTemperatureMeasurementAsync();

        testee.Uptime.Should().BePositive();
    }

    [Test]
    public void TrySetLowerTemperatureThreshold()
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        var result = testee.TrySetLowerTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.LowerTemperatureThreshold.Should().Be(35);
    }

    [TestCase(50, 40)]
    [TestCase(50, 50)]
    public void TrySetLowerTemperatureThresholdFailsForTooHighTemperature(int newLowerTemperatureThreshold, int currentUpperTemperatureThreshold)
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = currentUpperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        var result = testee.TrySetLowerTemperatureThreshold(newLowerTemperatureThreshold);

        result.Should().BeFalse();
        testee.LowerTemperatureThreshold.Should().Be(30);
    }

    [Test]
    public void SetAutomaticTemperatureRegulation()
    {
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();
        testee.SetManualTemperatureRegulation(false);

        testee.SetAutomaticTemperatureRegulation();

        testee.RegulationMode.Should().Be(RegulationMode.Automatic);
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void SetManualTemperatureRegulation(bool fanShouldRun, bool isFanRunning)
    {
        _fanControllerMock.IsFanRunning.Returns(isFanRunning);
        _optionsMonitorMock.CurrentValue
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = CreateTestee();

        testee.SetManualTemperatureRegulation(fanShouldRun);

        testee.RegulationMode.Should().Be(RegulationMode.Manual);
        if (fanShouldRun)
        {
            if (isFanRunning)
                _fanControllerMock.DidNotReceive().TurnFanOn();
            else
                _fanControllerMock.Received(1).TurnFanOn();
        }
        else
        {
            if (isFanRunning)
                _fanControllerMock.Received(1).TurnFanOff();
            else
                _fanControllerMock.DidNotReceive().TurnFanOff();
        }
    }

    private RaspiTemperatureController CreateTestee() =>
        new(_temperatureProviderMock,
            _fanControllerMock,
            _taskCancellationHelperMock,
            _taskHelperMock,
            Substitute.For<ILogger<RaspiTemperatureController>>(),
            _optionsMonitorMock);
}