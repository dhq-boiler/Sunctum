using ChinhDo.Transactions;
using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            var authors = LibraryManager.Value.AuthorManager.Authors;

            foreach (var author in authors)
            {
                if (author.NameIsEncrypted.Value)
                {
                    sequence.Add(async () =>
                    {
                        using (var dataOpUnit = new DataOperationUnit())
                        {
                            dataOpUnit.Open(ConnectionManager.DefaultConnection);
                            dataOpUnit.BeginTransaction();
                            try
                            {
                                var plainText = author.Name;
                                author.Name = await Encryptor.DecryptString(author.Name, Configuration.ApplicationConfiguration.Password);
                                author.NameIsEncrypted.Value = false;
                                AuthorFacade.Update(author, dataOpUnit);
                                dataOpUnit.Commit();
                            }
                            catch (Exception)
                            {
                                dataOpUnit.Rollback();
                            }
                        }
                    });
                }
            }

            foreach (var book in books)
            {
                if (book.TitleIsEncrypted.Value)
                {
                    sequence.Add(async () =>
                    {
                        using (var dataOpUnit = new DataOperationUnit())
                        {
                            dataOpUnit.Open(ConnectionManager.DefaultConnection);
                            dataOpUnit.BeginTransaction();
                            try
                            {
                                var plainText = book.Title;
                                if (book.TitleIsDecrypted.Value)
                                {
                                    //Do nothing
                                }
                                else
                                {
                                    var _book = book;
                                    BookFacade.FillContents(ref _book);
                                    book.Title = await Encryptor.DecryptString(_book.Title, Configuration.ApplicationConfiguration.Password);
                                }
                                book.TitleIsEncrypted.Value = false;
                                BookFacade.Update(book, dataOpUnit);
                                dataOpUnit.Commit();
                            }
                            catch (Exception)
                            {
                                dataOpUnit.Rollback();
                            }
                        }
                    });
                }

                var pages = book.Contents;
                foreach (var page in pages)
                {
                    if (page.Image.IsEncrypted)
                    {
                        sequence.Add(async () =>
                        {
                            using (var dataOpUnit = new DataOperationUnit())
                            {
                                await dataOpUnit.OpenAsync(ConnectionManager.DefaultConnection);
                                await dataOpUnit.BeginTransactionAsync();
                                try
                                {
                                    using (var scope = new TransactionScope())
                                    {
                                        _TargetEncryptImage = await EncryptImageFacade.FindByAsync(page.Image.ID, dataOpUnit);
                                        if (_TargetEncryptImage is not null)
                                        {
                                            var fileMgr = new TxFileManager();
                                            Encryptor.Decrypt(_TargetEncryptImage.EncryptFilePath, page.Image.AbsoluteMasterPath, Password, fileMgr);
                                            fileMgr.Delete(_TargetEncryptImage.EncryptFilePath);
                                            await EncryptImageFacade.DeleteBy(page.Image.ID, dataOpUnit);
                                        }
                                        if (page.Image.Thumbnail is not null)
                                        {
                                            ThumbnailFacade.DeleteWhereIDIs(page.Image.Thumbnail.ID, dataOpUnit);
                                        }
                                        page.Image.Thumbnail = null;
                                        page.Image.IsEncrypted = false;
                                        await ImageFacade.UpdateAsync(page.Image, dataOpUnit);
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
                                        await ImageFacade.UpdateAsync(page.Image, dataOpUnit);
                                        dataOpUnit.Commit();
                                    }
                                    else
                                    {
                                        dataOpUnit.Rollback();
                                    }
                                }
                            }
                        });
                    }

                    if (page.TitleIsEncrypted.Value)
                    {
                        sequence.Add(async () =>
                        {
                            using (var dataOpUnit = new DataOperationUnit())
                            {
                                dataOpUnit.Open(ConnectionManager.DefaultConnection);
                                dataOpUnit.BeginTransaction();
                                try
                                {
                                    var plainText = page.Title;
                                    if (page.TitleIsDecrypted.Value)
                                    {
                                        //Do nothing
                                    }
                                    else
                                    {
                                        var _page = page;
                                        PageFacade.GetProperty(ref _page);
                                        page.Title = await Encryptor.DecryptString(_page.Title, Configuration.ApplicationConfiguration.Password);
                                    }
                                    page.TitleIsEncrypted.Value = false;
                                    await PageFacade.UpdateAsync(page, dataOpUnit);
                                    dataOpUnit.Commit();
                                }
                                catch (Exception)
                                {
                                    dataOpUnit.Rollback();
                                }
                            }
                        });
                    }

                    var image = page.Image;
                    if (image.TitleIsEncrypted.Value)
                    {
                        sequence.Add(async () =>
                        {
                            using (var dataOpUnit = new DataOperationUnit())
                            {
                                dataOpUnit.Open(ConnectionManager.DefaultConnection);
                                dataOpUnit.BeginTransaction();
                                try
                                {
                                    var plainText = image.Title;
                                    if (image.TitleIsDecrypted.Value)
                                    {
                                        //Do nothing
                                    }
                                    else
                                    {
                                        var _image = image;
                                        ImageFacade.GetProperty(ref _image);
                                        image.Title = await Encryptor.DecryptString(_image.Title, Configuration.ApplicationConfiguration.Password);
                                    }
                                    image.TitleIsEncrypted.Value = false;
                                    await ImageFacade.UpdateAsync(image, dataOpUnit);
                                    dataOpUnit.Commit();
                                }
                                catch (Exception)
                                {
                                    dataOpUnit.Rollback();
                                }
                            }
                        });
                    }
                }
            }
            sequence.Add(() =>
            {
                var dao = new KeyValueDao();
                var record = dao.FindBy(new System.Collections.Generic.Dictionary<string, object>() { { "Key", "LibraryID" } }).SingleOrDefault();
                var libraryId = record?.Value;
                PasswordManager.RemovePassword(libraryId, Environment.UserName);
            });
            sequence.Add(() => OnmemoryImageManager.Instance.Clear());
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = null);
            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = false);
            sequence.Add(() => mainWindowViewModel.Value.Terminate());
            sequence.Add(() => mainWindowViewModel.Value.Initialize1stPhase(false));
            sequence.Add(() => mainWindowViewModel.Value.Initialize2ndPhase(false, false));
            sequence.Add(async () => await mainWindowViewModel.Value.Initialize3rdPhase().ConfigureAwait(false));
        }
    }
}
