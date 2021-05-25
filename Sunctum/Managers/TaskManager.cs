

using Ninject;
using NLog;
using Sunctum.Domail.Util;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static Sunctum.Domain.Models.Managers.ITaskManager;

namespace Sunctum.Managers
{
    public class TaskManager : ITaskManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private object _lockObj = new object();

        public ConcurrentQueue<AsyncTaskSequence> Sequences { get; } = new ConcurrentQueue<AsyncTaskSequence>();

        public AsyncTaskSequence CurrentSequence { get; private set; }

        public bool IsRunning { get; private set; }

        [Inject]
        public IProgressManager ProgressManager { get; set; }

        public event ExceptionOccurredEventHandler<Exception> ExceptionOccurred;

        public TaskManager()
        {
            CurrentSequence = null;
            IsRunning = false;
        }

        public async Task Enqueue(AsyncTaskSequence sequence)
        {
            Sequences.Enqueue(sequence);
            await Fire();
        }

        private async Task Fire()
        {
            await Task.Run(() => ProcessTaskLoop());
        }

        private void ProcessTaskLoop()
        {
            lock (_lockObj)
            {
                if (IsRunning)
                {
                    return;
                }

                IsRunning = true;
            }

            try
            {
                while (TakeSequence())
                {
                    InnerProcessTask(CurrentSequence);
                    CurrentSequence.Dispose();
                    CurrentSequence = null;
                }
            }
            catch (Exception e)
            {
                if (ExceptionOccurred != null)
                {
                    ExceptionOccurred(this, new Sunctum.Domain.Models.Managers.ExceptionOccurredEventArgs(e));
                }
            }
            finally
            {
                lock (_lockObj)
                {
                    IsRunning = false;
                }
            }
        }

        private bool TakeSequence()
        {
            if (CurrentSequence == null)
            {
                AsyncTaskSequence sequence = null;
                if (Sequences.TryDequeue(out sequence))
                {
                    CurrentSequence = sequence;
                    return true;
                }
                return false;
            }
            return true;
        }

        private void InnerProcessTask(AsyncTaskSequence sequence)
        {
            var tasks = sequence.Tasks.ToList();
            int total = tasks.Count;

            var timekeeper = new TimeKeeper();
            try
            {
                for (int i = 0; i < total; ++i)
                {
                    ProgressManager.UpdateProgress(i, total, timekeeper);

                    var t = tasks[i];

                    t.RunSynchronously();

                    if (t.Exception?.InnerExceptions?.Count > 0)
                    {
                        throw t.Exception;
                    }

                    if (total < tasks.Count)
                    {
                        total = tasks.Count;
                    }
                }

                ProgressManager.Complete();
            }
            catch (Exception e)
            {
                ProgressManager.Abort();
                s_logger.Error($"TaskManager aborted by:{e}");
                throw;
            }
        }
    }
}
