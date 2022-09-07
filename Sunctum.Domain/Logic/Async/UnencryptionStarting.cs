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
            var dataOpUnit = new DataOperationUnit();

            foreach (var book in books)
            {
                var pages = book.Contents;
                foreach (var page in pages)
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
                                _TargetEncryptImage = EncryptImageFacade.FindBy(page.Image.ID, dataOpUnit);
                                if (_TargetEncryptImage is not null)
                                {
                                    var fileMgr = new TxFileManager();
                                    Encryptor.Decrypt(_TargetEncryptImage.EncryptFilePath, page.Image.AbsoluteMasterPath, Password, fileMgr);
                                    fileMgr.Delete(_TargetEncryptImage.EncryptFilePath);
                                    EncryptImageFacade.DeleteBy(page.Image.ID, dataOpUnit);
                                }
                                if (page.Image.Thumbnail is not null)
                                {
                                    ThumbnailFacade.DeleteWhereIDIs(page.Image.Thumbnail.ID, dataOpUnit);
                                }
                                page.Image.Thumbnail = null;
                                page.Image.IsEncrypted = false;
                                ImageFacade.Update(page.Image, dataOpUnit);
                                scope.Complete();
                            }
                            dataOpUnit.Commit();
                        }
                        catch (Exception)
                        {
                            if (File.Exists(page.Image.AbsoluteMasterPath))
                            {
                                EncryptImageFacade.DeleteBy(page.Image.ID, dataOpUnit);
                                if (page.Image.Thumbnail is not null)
                                {
                                    ThumbnailFacade.DeleteWhereIDIs(page.Image.Thumbnail.ID, dataOpUnit);
                                }
                                page.Image.Thumbnail = null;
                                page.Image.IsEncrypted = false;
                                ImageFacade.Update(page.Image, dataOpUnit);
                                dataOpUnit.Commit();
                            }
                            else
                            {
                                dataOpUnit.Rollback();
                            }
                        }
                    });
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
