

using NLog;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Sunctum.Managers
{
    internal class FillContentsTaskManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<Guid, byte> _dic;

        public FillContentsTaskManager()
        {
            _dic = new ConcurrentDictionary<Guid, byte>();
        }

        internal async Task RunAsync(Action<BookViewModel> act, BookViewModel book)
        {
            if (_dic.ContainsKey(book.ID))
            {
                //Do nothing
                s_logger.Debug($"Order to fill Contents, but ignored. Book(ID={book.ID})");
            }
            else
            {
                await Task.Run(() => RunInternal(act, book));
            }
        }

        internal void Run(Action<BookViewModel> act, BookViewModel book)
        {
            if (_dic.ContainsKey(book.ID))
            {
                //Do nothing
                s_logger.Debug($"Order to fill Contents, but ignored. Book(ID={book.ID})");
            }
            else
            {
                RunInternal(act, book);
            }
        }

        private void RunInternal(Action<BookViewModel> act, BookViewModel book)
        {
            try
            {
                _dic.AddOrUpdate(book.ID, 0, new Func<Guid, byte, byte>((g, r) => { return r; }));
                act.Invoke(book);
            }
            finally
            {
                byte b;
                _dic.TryRemove(book.ID, out b);
            }
        }
    }
}
