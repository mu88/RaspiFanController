using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace RaspiFanController.Logic
{
    [ExcludeFromCodeCoverage]
    public class TaskCancellationHelper : ITaskCancellationHelper
    {
        public TaskCancellationHelper()
        {
            CancellationToken = new CancellationToken();
        }

        public bool IsCancellationRequested => CancellationToken.IsCancellationRequested;

        public CancellationToken CancellationToken { get; private set; }

        /// <inheritdoc />
        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }
}