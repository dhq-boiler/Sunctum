﻿

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryLoading : AsyncTaskMakerBase, ILibraryLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public IAuthorManager AuthorManager { get; set; }

        [Inject]
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public ITagManager TagManager { get; set; }

        [Inject]
        public ITaskManager TaskManager { get; set; }

        [Inject]
        public ILibraryResetting LibraryResettingService { get; set; }

        [Inject]
        public IRecentOpenedLibraryUpdating RecentOpenedLibraryUpdating { get; set; }

        [Inject]
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
