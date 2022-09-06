using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class UnencryptionStarting : AsyncTaskMakerBase, IUnencryptionStarting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public ILibraryResetting libraryResetting { get; set; }

        [Dependency]
        public ITaskManager taskManager { get; set; }

        [Dependency]
        public Lazy<IMainWindowViewModel> mainWindowViewModel { get; set; }

        public string Password { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start Unencryption"));
        }

        private EncryptImage _TargetEncryptImage;

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            taskManager.Enqueue(libraryResetting.GetTaskSequence());
            LibraryManager.Value.BookSource.AddRange(BookFacade.FindAllWithFillContents(null));

            var books = LibraryManager.Value.BookSource;

            foreach (var book in books)
            {
                var pages = book.Contents;
                foreach (var page in pages)
                {
                    sequence.Add(() => _TargetEncryptImage = EncryptImageFacade.FindBy(page.Image.ID));
                    sequence.Add(() => Encryptor.Decrypt(_TargetEncryptImage.EncryptFilePath, page.Image.AbsoluteMasterPath, Password));
                    sequence.Add(() => File.Delete($"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}"));
                    sequence.Add(() => EncryptImageFacade.DeleteBy(page.Image.ID));
                    sequence.Add(() => ThumbnailFacade.DeleteWhereIDIs(page.Image.Thumbnail.ID));
                    sequence.Add(() => page.Image.Thumbnail = null);
                }
            }
            sequence.Add(() => PasswordManager.RemovePassword(Environment.UserName));
            sequence.Add(() => OnmemoryImageManager.Instance.Clear());
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = null);
            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = false);
            sequence.Add(() => mainWindowViewModel.Value.Terminate());
            sequence.Add(() => mainWindowViewModel.Value.Initialize(false, false));
        }
    }
}
