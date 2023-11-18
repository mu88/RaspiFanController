using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class TaskHelper : ITaskHelper
{
    /// <inheritdoc />
    public Task DelayAsync(int millisecondsDelay, CancellationToken cancellationToken) => Task.Delay(millisecondsDelay, cancellationToken);
}