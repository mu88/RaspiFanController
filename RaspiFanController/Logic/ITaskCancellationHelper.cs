namespace RaspiFanController.Logic;

public interface ITaskCancellationHelper
{
    bool IsCancellationRequested { get; }

    CancellationToken CancellationToken { get; }

    void SetCancellationToken(CancellationToken cancellationToken);
}