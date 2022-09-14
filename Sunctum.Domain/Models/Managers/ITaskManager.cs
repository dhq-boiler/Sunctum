

using Sunctum.Domain.Logic.Async;
using System;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public delegate void ExceptionOccurredEventHandler<Exception>(object sender, ExceptionOccurredEventArgs args);

    public interface ITaskManager
    {
        AsyncTaskSequence CurrentSequence { get; }

        Task Enqueue(AsyncTaskSequence sequence);

        void WaitUntilProcessAll(TimeSpan? waitUnit = null);

        IProgressManager ProgressManager { get; }

        event ExceptionOccurredEventHandler<Exception> ExceptionOccurred;
    }
}
