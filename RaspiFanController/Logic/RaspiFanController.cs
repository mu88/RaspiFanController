using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class RaspiFanController : IFanController
{
    public RaspiFanController(ILogger<RaspiFanController> logger, IOptionsMonitor<AppSettings> settings)
    {
        Logger = logger;
        GpioPin = settings.CurrentValue.GpioPin;

        var gpioController = new GpioController();
        gpioController.OpenPin(GpioPin, PinMode.Input);
        var initialValue = gpioController.Read(GpioPin) == PinValue.High;
        IsFanRunning = initialValue;

        logger.LogInformation($"Initial value: {initialValue}");
    }

    /// <inheritdoc />
    public bool IsFanRunning { get; private set; }

    private ILogger<RaspiFanController> Logger { get; }

    private int GpioPin { get; }

    /// <inheritdoc />
    public void TurnFanOn()
    {
        var gpioController = new GpioController();
        gpioController.OpenPin(GpioPin, PinMode.Output);
        gpioController.Write(GpioPin, PinValue.High);
        IsFanRunning = true;

        Logger.LogInformation("Fan turned on");
    }

    /// <inheritdoc />
    public void TurnFanOff()
    {
        var gpioController = new GpioController();
        gpioController.OpenPin(GpioPin, PinMode.Output);
        gpioController.Write(GpioPin, PinValue.Low);
        IsFanRunning = false;

        Logger.LogInformation("Fan turned off");
    }
}