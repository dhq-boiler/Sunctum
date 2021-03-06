﻿

using Ninject;
using NLog;
using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryResetting : AsyncTaskMakerBase, IAsyncTaskMaker, ILibraryResetting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ILibrary LibraryManager { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start LibraryResetting"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => CleanUp());
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish LibraryResetting"));
        }

        private void CleanUp()
        {
            if (LibraryManager.BookSource != null)
            {
                foreach (var book in LibraryManager.BookSource)
                {
                    book.Dispose();
                }
                LibraryManager.BookSource.Clear();
            }
        }
    }
}
