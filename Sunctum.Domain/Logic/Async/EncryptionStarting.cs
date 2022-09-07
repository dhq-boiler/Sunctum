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
    public class EncryptionStarting : AsyncTaskMakerBase, IEncryptionStarting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public string Password { get; set; }

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public ILibraryResetting libraryResetting { get; set; }

        [Dependency]
        public ITaskManager taskManager { get; set; }

        [Dependency]
        public Lazy<IMainWindowViewModel> mainWindowViewModel { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start Encryption"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            taskManager.Enqueue(libraryResetting.GetTaskSequence());
            LibraryManager.Value.BookSource.AddRange(BookFacade.FindAllWithFillContents(null));

            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = true);
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = Password);
            sequence.Add(() => Directory.Delete($"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.CACHE_DIRECTORY}", true));
            sequence.Add(() => PasswordManager.SetPassword(Password, Environment.UserName));

            var books = LibraryManager.Value.BookSource;

            foreach (var book in books)
            {
                var images = book.Contents;
                foreach (var page in images)
                {
                    sequence.Add(() =>
                    {
                        var dirPath = $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}";
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                    });
                    sequence.Add(() => Encryptor.Encrypt(page.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", Password));
                    sequence.Add(() => Encryptor.DeleteOriginal(page));
                    sequence.Add(() => page.Image.IsEncrypted = true);
                    sequence.Add(() => ImageFacade.Update(page.Image));
                }
            }
            sequence.Add(() => mainWindowViewModel.Value.Terminate());
            sequence.Add(() => mainWindowViewModel.Value.Initialize(false, false));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish Encryption"));
        }
    }
}
