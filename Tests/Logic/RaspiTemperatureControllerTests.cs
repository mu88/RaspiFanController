using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RaspiFanController.Logic;

namespace Tests.Logic
{
    public class RaspiTemperatureControllerTests
    {
        [Test]
        public void SetSleepTime()
        {
            var testee = new AutoMocker().CreateInstance<RaspiTemperatureController>();

            testee.SetSleepTime(10);

            testee.MinimumSleepTime.Should().Be(10);
        }

        [Test]
        public async Task TurnFanOnInAutomaticMode()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((61, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(false);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, TimeSpan>(x => x.Elapsed).Returns(new TimeSpan(0, 2, 0));
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Once);
            autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshInterval, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task StartStopWatch()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((61, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(false);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(false);
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Never);
            autoMocker.Verify<IWrappedStopwatch>(x => x.Restart(), Times.Once);
            autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshInterval, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task TurnFanOffInAutomaticMode()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((39, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, TimeSpan>(x => x.Elapsed).Returns(new TimeSpan(0, 2, 0));
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Once);
            autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshInterval, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeriveUnitFromMeasuredTemperature()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((39, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, TimeSpan>(x => x.Elapsed).Returns(new TimeSpan(0, 2, 0));
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            testee.Unit.Should().Be("C");
        }

        [Test]
        public void IsPlatformSupported()
        {
            var autoMocker = new AutoMocker();
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            // ReSharper disable once UnusedVariable - Reviewed: the result is not necessary, only the call is important
            var result = testee.IsPlatformSupported;

            autoMocker.Verify<ITemperatureProvider, bool>(x => x.IsPlatformSupported(), Times.Once);
        }

        [Test]
        public async Task DoNotTurnFanOffInAutomaticModeWhenSleepTimeNotReached()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((39, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, TimeSpan>(x => x.Elapsed).Returns(new TimeSpan(0, 0, 20));
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            autoMocker.Verify<IFanController>(x => x.TurnFanOff(), Times.Never);
            autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshInterval, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DoNotTurnFanOnInAutomaticModeWhenSleepTimeNotReached()
        {
            var autoMocker = new AutoMocker();
            autoMocker.GetMock<ITaskCancellationHelper>().SetupSequence(x => x.IsCancellationRequested).Returns(false).Returns(true);
            autoMocker.Setup<ITemperatureProvider, (double, string)>(x => x.GetTemperature()).Returns((51, "C"));
            autoMocker.Setup<IFanController, bool>(x => x.IsFanRunning).Returns(false);
            autoMocker.Setup<IWrappedStopwatch, bool>(x => x.IsRunning).Returns(true);
            autoMocker.Setup<IWrappedStopwatch, TimeSpan>(x => x.Elapsed).Returns(new TimeSpan(0, 0, 20));
            var testee = autoMocker.CreateInstance<RaspiTemperatureController>();

            await testee.StartTemperatureMeasurementAsync();

            autoMocker.Verify<IFanController>(x => x.TurnFanOn(), Times.Never);
            autoMocker.Verify<ITaskHelper>(x => x.Delay(testee.RefreshInterval, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void SetTemperatureThreshold()
        {
            var testee = new AutoMocker().CreateInstance<RaspiTemperatureController>();

            testee.SetTemperatureThreshold(10);

            testee.TemperatureThreshold.Should().Be(10);
        }

        [Test]
        public void SetAutomaticTemperatureRegulation()
        {
            var testee = new AutoMocker().CreateInstance<RaspiTemperatureController>();

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
}