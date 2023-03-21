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
            taskManager.RunSync(libraryResetting.GetTaskSequence());
            LibraryManager.Value.BookSource.AddRange(BookFacade.FindAllWithFillContents(null));

            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = true);
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = Password);
            sequence.Add(() =>
            {
                var path = $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.CACHE_DIRECTORY}";
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            });
            sequence.Add(() =>
            {
                var dao = new KeyValueDao();
                var record = dao.FindBy(new System.Collections.Generic.Dictionary<string, object>() { { "Key", "LibraryID" } }).SingleOrDefault();
                var libraryId = record?.Value;
                PasswordManager.SetPassword(libraryId, Password, Environment.UserName);
            });

            var authors = LibraryManager.Value.AuthorManager.Authors;

            foreach (var author in authors)
            {
                if (!author.NameIsEncrypted.Value)
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
                                author.Name = await Encryptor.EncryptString(author.Name, Configuration.ApplicationConfiguration.Password);
                                author.NameIsEncrypted.Value = true;
                                AuthorFacade.Update(author, dataOpUnit);
                                dataOpUnit.Commit();
                                author.Name = plainText;
                                author.NameIsDecrypted.Value = true;
                            }
                            catch (Exception)
                            {
                                dataOpUnit.Rollback();
                            }
                        }
                    });
                }
            }

            var books = LibraryManager.Value.BookSource;

            foreach (var book in books)
            {
                sequence.Add(async () =>
                {
                    try
                    {
                        using (var dataOpUnit = new DataOperationUnit())
                        using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(3)))
                        {
                            await dataOpUnit.OpenAsync(ConnectionManager.DefaultConnection);
                            await dataOpUnit.BeginTransactionAsync();

                            try
                            {
                                if (!book.TitleIsEncrypted.Value)
                                {
                                    var plainText = book.Title;
                                    book.Title = await Encryptor.EncryptString(book.Title, Configuration.ApplicationConfiguration.Password);
                                    book.TitleIsEncrypted.Value = true;
                                    await BookFacade.Update(book, dataOpUnit);
                                    book.Title = plainText;
                                    book.TitleIsDecrypted.Value = true;
                                }

                                var images = book.Contents;
                                foreach (var page in images)
                                {
                                    if (!page.Image.IsEncrypted)
                                    {
                                        var fileMgr = new TxFileManager();
                                        var dirPath = $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}";
                                        if (!fileMgr.DirectoryExists(dirPath))
                                        {
                                            fileMgr.CreateDirectory(dirPath);
                                        }
                                        Encryptor.Encrypt(page.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", Password, dataOpUnit, fileMgr);
                                        Encryptor.DeleteOriginal(page, fileMgr);
                                        page.Image.IsEncrypted = true;
                                        await ImageFacade.UpdateAsync(page.Image, dataOpUnit);
                                    }

                                    if (!page.TitleIsEncrypted.Value)
                                    {
                                        var plainText = page.Title;
                                        page.Title = await Encryptor.EncryptString(page.Title, Configuration.ApplicationConfiguration.Password);
                                        page.TitleIsEncrypted.Value = true;
                                        await PageFacade.UpdateAsync(page, dataOpUnit);
                                        page.Title = plainText;
                                        page.TitleIsDecrypted.Value = true;
                                    }

                                    var image = page.Image;
                                    if (!image.TitleIsEncrypted.Value)
                                    {
                                        var plainText = image.Title;
                                        image.Title = await Encryptor.EncryptString(image.Title, Configuration.ApplicationConfiguration.Password);
                                        image.TitleIsEncrypted.Value = true;
                                        await ImageFacade.UpdateAsync(image, dataOpUnit);
                                        image.Title = plainText;
                                        image.TitleIsDecrypted.Value = true;
                                    }
                                }
                                dataOpUnit.Commit();
                                scope.Complete();
                            }
                            catch (Exception e)
                            {
                                dataOpUnit.Rollback();
                            }
                        }
                    }
                    catch (TransactionAbortedException e)
                    {
                        s_logger.Error(e);
                    }
                });
            }
            sequence.Add(() => mainWindowViewModel.Value.Terminate());
            sequence.Add(() => mainWindowViewModel.Value.Initialize1stPhase(false));
            sequence.Add(() => mainWindowViewModel.Value.Initialize2ndPhase(false, false));
            sequence.Add(async () => await mainWindowViewModel.Value.Initialize3rdPhase().ConfigureAwait(false));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish Encryption"));
        }
    }
}
