using System.Threading;
using System.Threading.Tasks;

namespace RaspiFanController.Logic
{
    public interface ITaskHelper
    {
        Task Delay(int millisecondsDelay, CancellationToken cancellationToken);
    }
}