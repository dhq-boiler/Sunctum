

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Logic.PageSorting;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Sunctum.Managers
{
    public class Library : BookStorage, ILibrary
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<RecentOpenedLibrary> _RecentOpenedLibraryList;

        public Library()
        {
        }

        #region プロパティ

        public ObservableCollection<RecentOpenedLibrary> RecentOpenedLibraryList
        {
            [DebuggerStepThrough]
            get
            { return _RecentOpenedLibraryList; }
            set { SetProperty(ref _RecentOpenedLibraryList, value); }
        }

        [Dependency]
        public IPageOrderUpdating PageOrderUpdatingService { get; set; }

        [Dependency]
        public ITagManager TagManager { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IProgressManager ProgressManager { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IAuthorManager AuthorManager { [DebuggerStepThrough] get; set; }

        [Dependency]
        public ITaskManager TaskManager { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IBookExporting BookExportingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IBookImporting BookImportingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IBookRemoving BookRemovingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public ILibraryImporting LibraryImportingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public ILibraryLoading LibraryLoadingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IPageRemoving PageRemovingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IPageScrapping PageScrappingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IBookThumbnailRemaking BookThumbnailRemakingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IPageThumbnailRemaking PageThumbnailRemakingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public ILibraryInitializing LibraryInitializingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public ILibraryResetting LibraryResettingService { [DebuggerStepThrough] get; set; }

        [Dependency]
        public IByteSizeCalculating ByteSizeCalculatingService { get; set; }

        [Dependency]
        public IBookHashing BookHashingService { get; set; }

        [Dependency]
        public IBookTagInitializing BookTagInitializingService { get; set; }

        [Dependency]
        public IEncryptionStarting EncryptionStartingService { get; set; }

        [Dependency]
        public IUnencryptionStarting UnencryptionStartingService { get; set; }

        [Dependency]
        public IDataAccessManager DataAccessManager { [DebuggerStepThrough] get; set; }

        #endregion //プロパティ

        public async Task Reset()
        {
            await TaskManager.Enqueue(LibraryResettingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #region 初期化&ロード

        public async Task Initialize()
        {
            TaskManager.ExceptionOccurred += TaskManager_ExceptionOccurred;
            await TaskManager.Enqueue(LibraryInitializingService.GetTaskSequence()).ConfigureAwait(false);
            TaskManager.ExceptionOccurred -= TaskManager_ExceptionOccurred;
        }

        private void TaskManager_ExceptionOccurred(object sender, Domain.Models.Managers.ExceptionOccurredEventArgs args)
        {
            s_logger.Fatal(args.Exception, "Failed to initialize database.");
            MessageBox.Show("データベースの初期化に失敗しました．\nSunctumを終了します．", "データベース初期化エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            throw args.Exception;
        }

        public async Task Load()
        {
            try
            {
                await TaskManager.Enqueue(LibraryLoadingService.GetTaskSequence()).ConfigureAwait(false);
            }
            catch (FailedOpeningDatabaseException e)
            {
                s_logger.Fatal("Failed to open database．Sunctum will be terminated．", e);
                MessageBox.Show("データベースの接続に失敗しました．\nSunctumを終了します．", "データベース接続エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        #endregion //初期化&ロード

        #region インポート

        public async Task ImportAsync(string[] objectPaths)
        {
            try
            {
                BookImportingService.MasterDirectory = Specifications.MASTER_DIRECTORY;
                BookImportingService.ObjectPaths = objectPaths;
                await TaskManager.Enqueue(BookImportingService.GetTaskSequence()).ConfigureAwait(false);
            }
            catch (NullReferenceException e)
            {
                s_logger.Error(e, "Failed to import.");
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Failed to import.");
            }
        }

        public async Task ImportLibrary(string directory)
        {
            try
            {
                string importLibraryDbFilename = Specifications.GenerateAbsoluteLibraryDbFilename(directory);

                LibraryImportingService.ImportLibraryFilename = importLibraryDbFilename;
                await TaskManager.Enqueue(LibraryImportingService.GetTaskSequence()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Failed to import.");
            }
        }

        #endregion //インポート

        #region エンティティ操作

        #region インターフェース

        public async Task RemoveBooks(BookViewModel[] books)
        {
            BookRemovingService.TargetBooks = books;
            await TaskManager.Enqueue(BookRemovingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public async Task RemovePages(PageViewModel[] pages)
        {
            PageRemovingService.TargetPages = pages;
            await TaskManager.Enqueue(PageRemovingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //インターフェース

        #endregion //エンティティ操作

        #region ソート

        public BookViewModel OrderForward(PageViewModel page, BookViewModel book)
        {
            return PageOrdering.OrderForward(page, book);
        }

        public BookViewModel OrderBackward(PageViewModel page, BookViewModel book)
        {
            return PageOrdering.OrderBackward(page, book);
        }

        public async Task SaveBookContentsOrder(BookViewModel target)
        {
            PageOrderUpdatingService.Target = target;
            await TaskManager.Enqueue(PageOrderUpdatingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //ソート

        #region スクラップ

        public async Task ScrapPages(AuthorViewModel author, string title, PageViewModel[] pages)
        {
            PageScrappingService.Title = title;
            PageScrappingService.TargetPages = pages;
            PageScrappingService.MasterDirectory = Specifications.MASTER_DIRECTORY;
            await TaskManager.Enqueue(PageScrappingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //スクラップ

        #region エクスポート

        public async Task ExportBooks(BookViewModel[] books, string directory, bool tag)
        {
            BookExportingService.TargetBooks = books;
            BookExportingService.DestinationDirectory = directory;
            BookExportingService.IncludeTag = tag;
            await TaskManager.Enqueue(BookExportingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //エクスポート

        #region サムネイル再作成

        public async Task RemakeThumbnail(IEnumerable<BookViewModel> books)
        {
            BookThumbnailRemakingService.TargetBooks = books;
            await TaskManager.Enqueue(BookThumbnailRemakingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public async Task RemakeThumbnail(IEnumerable<PageViewModel> pages)
        {
            PageThumbnailRemakingService.TargetPages = pages;
            await TaskManager.Enqueue(PageThumbnailRemakingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //サムネイル再作成

        #region サイズ更新

        public async Task UpdateBookByteSizeAll()
        {
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsAll;
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public async Task UpdateBookByteSizeStillNull()
        {
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsStillNull;
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //サイズ更新

        #region 指紋更新

        public async Task UpdateBookFingerPrintAll()
        {
            BookHashingService.Range = BookHashing.UpdateRange.IsAll;
            await TaskManager.Enqueue(BookHashingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public async Task UpdateBookFingerPrintStillNull()
        {
            BookHashingService.Range = BookHashing.UpdateRange.IsStillNull;
            await TaskManager.Enqueue(BookHashingService.GetTaskSequence()).ConfigureAwait(false);
        }

        #endregion //指紋更新

        public async Task UpdateBookTag()
        {
            await TaskManager.Enqueue(BookTagInitializingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public IArrangedBookStorage CreateBookStorage()
        {
            return new BookCabinet(this.BookSource);
        }

        public async Task StartEncryption(string password)
        {
            EncryptionStartingService.Password = password;
            await TaskManager.Enqueue(EncryptionStartingService.GetTaskSequence()).ConfigureAwait(false);
        }

        public async Task<bool> UnlockIfLocked()
        {
            var encryptedItemsCount = await EncryptImageFacade.CountAllAsync();
            if (encryptedItemsCount == 0)
            {
                return false;
            }

            try
            {
                var dao = new KeyValueDao();
                var record = (await dao.FindByAsync(new System.Collections.Generic.Dictionary<string, object>() { { "Key", "LibraryID" } }).ToListAsync()).SingleOrDefault();
                var libraryId = record?.Value;
                var password = await PasswordManager.SignInAsync(libraryId, Environment.UserName).ConfigureAwait(false);
                if (password is not null)
                {
                    Configuration.ApplicationConfiguration.Password = password;
                    return true;
                }
                else
                {
                    return await CallInputPasswordDialog();
                }
            }
            catch (COMException e)
            {
                return await CallInputPasswordDialog();
            }
        }

        private static async Task<bool> CallInputPasswordDialog()
        {
            return (await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                InputPasswordDialog dialog = new InputPasswordDialog("このライブラリは暗号化されています。閲覧するにはパスワードが必要です。");

                if (dialog.ShowDialog() == true)
                {
                    var dao = new KeyValueDao();
                    var record = (await dao.FindByAsync(new System.Collections.Generic.Dictionary<string, object>() { { "Key", "LibraryID" } }).ToListAsync()).SingleOrDefault();
                    var libraryId = record?.Value;
                    Configuration.ApplicationConfiguration.Password = dialog.Password;
                    PasswordManager.SetPassword(libraryId, dialog.Password, Environment.UserName);
                    return true;
                }
                else
                {
                    return false;
                }
            })).Result;
        }

        public async Task StartUnencryption(string password)
        {
            UnencryptionStartingService.Password = password;
            await TaskManager.Enqueue(UnencryptionStartingService.GetTaskSequence()).ConfigureAwait(false);
        }
    }
}
