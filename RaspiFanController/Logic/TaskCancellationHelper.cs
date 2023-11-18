using System.Diagnostics.CodeAnalysis;

namespace RaspiFanController.Logic;

[ExcludeFromCodeCoverage]
public class TaskCancellationHelper : ITaskCancellationHelper
{
    public bool IsCancellationRequested => CancellationToken.IsCancellationRequested;

    public CancellationToken CancellationToken { get; private set; }

    /// <inheritdoc />
    public void SetCancellationToken(CancellationToken cancellationToken) => CancellationToken = cancellationToken;
}