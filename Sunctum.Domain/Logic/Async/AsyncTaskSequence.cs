

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class AsyncTaskSequence : IDisposable
    {
        private List<Task> _Tasks;

        public IEnumerable<Task> Tasks
        {
            get { return _Tasks; }
            set { _Tasks = new List<Task>(value); }
        }

        public int Count { get { return Tasks.Count(); } }

        public AsyncTaskSequence()
        {
            Tasks = new List<Task>();
        }

        public AsyncTaskSequence(IEnumerable<Task> tasks)
        {
            Debug.Assert(tasks != null);
            Tasks = tasks;
        }

        public void Add(Task task)
        {
            _Tasks.Add(task);
        }

        public void Add(Action action)
        {
            _Tasks.Add(new Task(action));
        }

        public void AddRange(IEnumerable<Task> tasks)
        {
            _Tasks.AddRange(tasks);
        }

        public void RemoveAt(int index)
        {
            _Tasks.RemoveAt(index);
        }

        public void Clear()
        {
            _Tasks.Clear();
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _Tasks.Clear();
                }

                _Tasks = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
