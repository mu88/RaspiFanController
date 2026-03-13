namespace RaspiFanController.Logic;

public interface ITemperatureProvider
{
    (double Temperature, string Unit) GetTemperature();

    bool IsPlatformSupported();
}
