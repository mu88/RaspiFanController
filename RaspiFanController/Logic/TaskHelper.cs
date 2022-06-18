using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

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