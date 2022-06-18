namespace RaspiFanController.Logic;

public interface ITaskHelper
{
    Task Delay(int millisecondsDelay, CancellationToken cancellationToken);
}