using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
internal sealed partial class RaspiFanController : IFanController, IDisposable
{
    private readonly ILogger<RaspiFanController> _logger;
    private readonly GpioController _gpioController = new();
    private readonly GpioPin _gpioPin;

    public RaspiFanController(ILogger<RaspiFanController> logger, IOptions<AppSettings> settings)
    {
        _logger = logger;
        GpioPin = settings.Value.GpioPin;

        _gpioPin = _gpioController.OpenPin(GpioPin, PinMode.Input);
        var initialValue = _gpioController.Read(GpioPin) == PinValue.High;
        _gpioController.SetPinMode(GpioPin, PinMode.Output);
        IsFanRunning = initialValue;

        LogInitialValue(initialValue);
    }

    /// <inheritdoc />
    public bool IsFanRunning { get; private set; }

    private int GpioPin { get; }

    /// <inheritdoc />
    public void TurnFanOn()
    {
        _gpioController.Write(GpioPin, PinValue.High);
        IsFanRunning = true;

        LogFanTurnedOn();
    }

    /// <inheritdoc />
    public void TurnFanOff()
    {
        _gpioController.Write(GpioPin, PinValue.Low);
        IsFanRunning = false;

        LogFanTurnedOff();
    }

    public void Dispose()
    {
        _gpioPin.Dispose();
        _gpioController.Dispose();
    }

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Initial value: {InitialValue}")]
    private partial void LogInitialValue(bool initialValue);

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Fan turned on")]
    private partial void LogFanTurnedOn();

    [LoggerMessage(Level = LogLevel.Information, SkipEnabledCheck = true, Message = "Fan turned off")]
    private partial void LogFanTurnedOff();
}