

using Sunctum.Domain.Logic.Async;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public interface ITaskManager
    {
        AsyncTaskSequence CurrentSequence { get; }

        Task Enqueue(AsyncTaskSequence sequence);

        IProgressManager ProgressManager { get; }
    }
}
