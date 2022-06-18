namespace RaspiFanController.Logic;

public interface IFanController
{
    bool IsFanRunning { get; }

    void TurnFanOn();

    void TurnFanOff();
}