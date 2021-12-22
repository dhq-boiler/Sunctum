using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryLoading : AsyncTaskMakerBase, ILibraryLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public IAuthorManager AuthorManager { get; set; }

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public ITagManager TagManager { get; set; }

        [Dependency]
        public ITaskManager TaskManager { get; set; }

        [Dependency]
        public ILibraryResetting LibraryResettingService { get; set; }

        [Dependency]
        public IRecentOpenedLibraryUpdating RecentOpenedLibraryUpdating { get; set; }

        [Dependency]
        public IBookLoading BookLoadingService { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start LibraryLoading"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => TaskManager.Enqueue(LibraryResettingService.GetTaskSequence()));

            sequence.Add(() => TaskManager.Enqueue(RecentOpenedLibraryUpdating.GetTaskSequence()));

            sequence.Add(() => TagManager.Load());

            sequence.Add(() => AuthorManager.LoadAsync());

            sequence.Add(() => TaskManager.Enqueue(BookLoadingService.GetTaskSequence()));

            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = EncryptImageFacade.AnyEncrypted());
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish LibraryLoading"));
        }
    }
}
