namespace RaspiFanController.Logic;

public interface ITaskHelper
{
    Task DelayAsync(int millisecondsDelay, CancellationToken cancellationToken);
}