using ChinhDo.Transactions;
using Homura.ORM;
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
using System.Transactions;
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
            var dataOpUnit = new DataOperationUnit();

            foreach (var book in books)
            {
                var images = book.Contents;
                foreach (var page in images)
                {
                    sequence.Add(() =>
                    {
                        dataOpUnit = new DataOperationUnit();
                        dataOpUnit.Open(ConnectionManager.DefaultConnection);
                        dataOpUnit.BeginTransaction();
                        try
                        {
                            using (var scope = new TransactionScope())
                            {
                                var fileMgr = new TxFileManager();
                                var dirPath = $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}";
                                if (!fileMgr.DirectoryExists(dirPath))
                                {
                                    fileMgr.CreateDirectory(dirPath);
                                }
                                Encryptor.Encrypt(page.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", Password, fileMgr);
                                Encryptor.DeleteOriginal(page, fileMgr);
                                page.Image.IsEncrypted = true;
                                ImageFacade.Update(page.Image, dataOpUnit);
                                scope.Complete();
                            }
                            dataOpUnit.Commit();
                        }
                        catch (Exception)
                        {
                            dataOpUnit.Rollback();
                        }
                    });
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
