using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class TaskHelper : ITaskHelper
{
    /// <inheritdoc />
    public Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        return Task.Delay(millisecondsDelay, cancellationToken);
    }
}