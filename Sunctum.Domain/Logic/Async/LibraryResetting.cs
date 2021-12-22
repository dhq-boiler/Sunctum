using NLog;
using Sunctum.Domain.Models.Managers;
using System;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryResetting : AsyncTaskMakerBase, IAsyncTaskMaker, ILibraryResetting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

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
            if (LibraryManager.Value.BookSource != null)
            {
                foreach (var book in LibraryManager.Value.BookSource)
                {
                    book.Dispose();
                }
                LibraryManager.Value.BookSource.Clear();
            }
        }
    }
}
