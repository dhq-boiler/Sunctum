

using Prism.Mvvm;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public abstract class AsyncTaskMakerBase : BindableBase, IAsyncTaskMaker
    {
        private AsyncTaskSequence Sequence { get; set; }

        public void CreateSequence()
        {
            Sequence = new AsyncTaskSequence();
        }

        public AsyncTaskSequence GetTaskSequence()
        {
            CreateSequence();
            ConfigurePreTaskAction(Sequence);
            ConfigureTaskImplementation(Sequence);
            ConfigurePostTaskAction(Sequence);
            return Sequence;
        }

        public virtual void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            //Do nothing
        }

        public abstract void ConfigureTaskImplementation(AsyncTaskSequence sequence);

        public virtual void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            //Do nothing
        }
    }
}
