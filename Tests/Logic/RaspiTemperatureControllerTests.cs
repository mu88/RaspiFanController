using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Logic;

public class RaspiTemperatureControllerTests
{
    [Test]
    public async Task TurnFanOnInAutomaticMode()
    {
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((61, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(false);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Once);
        autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshMilliseconds, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task TurnFanOffInAutomaticMode()
    {
        var autoMocker = new AutoMocker();
        autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
        autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((29, "C"));
        autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Once);
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
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        testee.Unit.Should().Be("C");
    }

    [Test]
    public void IsPlatformSupported()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
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
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetUpperTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.UpperTemperatureThreshold.Should().Be(35);
    }

    [Test]
    public void TrySetUpperTemperatureThresholdFailsForTooLowTemperature()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetUpperTemperatureThreshold(10);

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
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        await testee.StartTemperatureMeasurementAsync();

        testee.Uptime.Should().BePositive();
    }

    [Test]
    public void TrySetLowerTemperatureThreshold()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetLowerTemperatureThreshold(35);

        result.Should().BeTrue();
        testee.LowerTemperatureThreshold.Should().Be(35);
    }

    [Test]
    public void TrySetLowerTemperatureThresholdFailsForTooHighTemperature()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        var result = testee.TrySetLowerTemperatureThreshold(50);

        result.Should().BeFalse();
        testee.LowerTemperatureThreshold.Should().Be(30);
    }

    [Test]
    public void SetAutomaticTemperatureRegulation()
    {
        var autoMocker = new AutoMocker();
        autoMocker.Setup<IOptionsMonitor<AppSettings>, AppSettings>(x => x.CurrentValue)
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

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
            .Returns(new AppSettings
            {
                RefreshMilliseconds = 1, UpperTemperatureThreshold = 40, LowerTemperatureThreshold = 30
            });
        var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

        testee.SetManualTemperatureRegulation(fanShouldRun);

        testee.RegulationMode.Should().Be(RegulationMode.Manual);
        if (fanShouldRun)
        {
            if (isFanRunning)
            {
                autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Never);
            }
            else
            {
                autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Once);
            }
        }
        else
        {
            if (isFanRunning)
            {
                autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Once);
            }
            else
            {
                autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Never);
            }
        }
    }
}