using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
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
            sequence.Add(async () => await TaskManager.Enqueue(LibraryResettingService.GetTaskSequence()).ConfigureAwait(false));

            sequence.Add(async () => await TaskManager.Enqueue(RecentOpenedLibraryUpdating.GetTaskSequence()).ConfigureAwait(false));

            sequence.Add(async () => await TagManager.LoadAsync().ConfigureAwait(false));

            sequence.Add(async () => await AuthorManager.LoadAsync().ConfigureAwait(false));

            sequence.Add(async () => await TaskManager.Enqueue(BookLoadingService.GetTaskSequence()).ConfigureAwait(false));

            sequence.Add(async () => Configuration.ApplicationConfiguration.LibraryIsEncrypted = await EncryptImageFacade.AnyEncryptedAsync().ConfigureAwait(false));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish LibraryLoading"));
        }
    }
}
