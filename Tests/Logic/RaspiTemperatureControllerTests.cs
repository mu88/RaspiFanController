using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Logic;

public class RaspiTemperatureControllerTests
{
    [TestCase(61, 40)]
    [TestCase(61, 61)]
    public async Task TurnFanOnInAutomaticMode(int currentTemperature, int upperTemperatureThreshold)
    {
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((currentTemperature, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(false);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = upperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Once);
        autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshMilliseconds, It.IsAny<CancellationToken>()), Times.Once);
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
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((currentTemperature, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(fanIsRunning);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = lowerTemperatureThreshold
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        if (expectedFanToBeTurnedOff) autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Once);
        autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshMilliseconds, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeriveUnitFromMeasuredTemperature()
    {
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((39, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        testee.Unit.Should().Be("C");
    }

    [Test]
    public void IsPlatformSupported()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        // ReSharper disable once UnusedVariable - Reviewed: the result is not necessary, only the call is important
        var result = testee.IsPlatformSupported;

        autoMocker.Verify<ITemperatureProvider, bool>(x => x.IsPlatformSupported(), Times.Once);
    }

    [Test]
    public void TrySetUpperTemperatureThreshold()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetUpperTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.UpperTemperatureThreshold.Should().Be(35);
    }

    [TestCase(10, 30)]
    [TestCase(30, 30)]
    public void TrySetUpperTemperatureThresholdFailsForTooLowTemperature(int newUpperTemperatureThreshold, int currentLowerTemperatureThreshold)
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = currentLowerTemperatureThreshold });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetUpperTemperatureThreshold(newUpperTemperatureThreshold);

        result.Should().BeFalse();
        testee.UpperTemperatureThreshold.Should().Be(40);
    }

    [Test]
    public async Task GetUptime()
    {
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((39, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        testee.Uptime.Should().BePositive();
    }

    [Test]
    public void TrySetLowerTemperatureThreshold()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetLowerTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.LowerTemperatureThreshold.Should().Be(35);
    }

    [TestCase(50, 40)]
    [TestCase(50, 50)]
    public void TrySetLowerTemperatureThresholdFailsForTooHighTemperature(int newLowerTemperatureThreshold, int currentUpperTemperatureThreshold)
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = currentUpperTemperatureThreshold, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetLowerTemperatureThreshold(newLowerTemperatureThreshold);

        result.Should().BeFalse();
        testee.LowerTemperatureThreshold.Should().Be(30);
    }

    [Test]
    public void SetAutomaticTemperatureRegulation()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();
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
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(isFanRunning);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings { RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30 });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        testee.SetManualTemperatureRegulation(fanShouldRun);

        testee.RegulationMode.Should().Be(RegulationMode.Manual);
        if (fanShouldRun)
        {
            if (isFanRunning)
                autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Never);
            else
                autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Once);
        }
        else
        {
            if (isFanRunning)
                autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Once);
            else
                autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Never);
        }
    }
}