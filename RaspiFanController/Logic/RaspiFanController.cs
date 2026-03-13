using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public partial class RaspiFanController : IFanController
{
    private readonly ILogger<RaspiFanController> _logger;

    public RaspiFanController(ILogger<RaspiFanController> logger, IOptionsMonitor<AppSettings> settings)
    {
        _logger = logger;
        GpioPin = settings.CurrentValue.GpioPin;

        using var gpioController = new GpioController();
        using var openPin = gpioController.OpenPin(GpioPin, PinMode.Input);
        var initialValue = gpioController.Read(GpioPin) == PinValue.High;
        IsFanRunning = initialValue;

        LogInitialValue(initialValue);
    }

    /// <inheritdoc />
    public bool IsFanRunning { get; private set; }

    private int GpioPin { get; }

    /// <inheritdoc />
    public void TurnFanOn()
    {
        using var gpioController = new GpioController();
        using var openPin = gpioController.OpenPin(GpioPin, PinMode.Output);
        gpioController.Write(GpioPin, PinValue.High);
        IsFanRunning = true;

        _logger.LogInformation("Fan turned on");
    }

    /// <inheritdoc />
    public void TurnFanOff()
    {
        using var gpioController = new GpioController();
        using var openPin = gpioController.OpenPin(GpioPin, PinMode.Output);
        gpioController.Write(GpioPin, PinValue.Low);
        IsFanRunning = false;

        _logger.LogInformation("Fan turned off");
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Initial value: {InitialValue}")]
    private partial void LogInitialValue(bool initialValue);
}
