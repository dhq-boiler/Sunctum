﻿

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sunctum.Domain.Logic.Async
{
    public class BookLoading : AsyncTaskMakerBase, IAsyncTaskMaker, IBookLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private Stopwatch _stopwatch = new Stopwatch();

        [Inject]
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public IAuthorManager AuthorManager { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookLoading"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => _stopwatch.Reset());

            sequence.Add(() => s_logger.Info("Loading Book list..."));

            sequence.Add(() => _stopwatch.Start());

            sequence.Add(() => LibraryManager.BookSource.CollectionChanged -= AuthorManager.LoadedBooks_CollectionChanged);

            sequence.Add(() => LibraryManager.BookSource = new ObservableCollection<BookViewModel>(BookFacade.FindAllWithAuthor(null)));

            sequence.Add(() => LibraryManager.BookSource.CollectionChanged += AuthorManager.LoadedBooks_CollectionChanged);

            sequence.Add(() => _stopwatch.Stop());

            sequence.Add(() => s_logger.Info($"Completed to load Book list. {_stopwatch.ElapsedMilliseconds}ms"));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookLoading"));
        }
    }
}
