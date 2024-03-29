﻿using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class BookLoading : AsyncTaskMakerBase, IAsyncTaskMaker, IBookLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private Stopwatch _stopwatch = new Stopwatch();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
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

            sequence.Add(() => LibraryManager.Value.BookSource.CollectionChanged -= AuthorManager.LoadedBooks_CollectionChanged);

            sequence.Add(async () =>
            {
                LibraryManager.Value.BookSource.Clear();
                LibraryManager.Value.BookSource.AddRange(await BookFacade.FindAllWithAuthorAsync(null).ToListAsync());
            });

            sequence.Add(() => LibraryManager.Value.BookSource.CollectionChanged += AuthorManager.LoadedBooks_CollectionChanged);

            sequence.Add(() => _stopwatch.Stop());

            sequence.Add(() => s_logger.Info($"Completed to load Book list. {_stopwatch.ElapsedMilliseconds}ms"));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookLoading"));
        }
    }
}
