

using Homura.ORM;
using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.PageSorting;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Hosting;
using System.Threading.Tasks;
using System.Windows;

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

        [Inject]
        public ITagManager TagMng {[DebuggerStepThrough] get; set; }

        [Inject]
        public IProgressManager ProgressManager {[DebuggerStepThrough] get; set; }

        [Inject]
        public IAuthorManager AuthorManager {[DebuggerStepThrough] get; set; }

        [Inject]
        public ITaskManager TaskManager {[DebuggerStepThrough] get; set; }

        [Inject]
        public IBookExporting BookExportingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IBookImporting BookImportingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IBookRemoving BookRemovingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public ILibraryImporting LibraryImportingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public ILibraryLoading LibraryLoadingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IPageRemoving PageRemovingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IPageScrapping PageScrappingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IBookThumbnailRemaking BookThumbnailRemakingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IPageThumbnailRemaking PageThumbnailRemakingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public ILibraryInitializing LibraryInitializingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public ILibraryResetting LibraryResettingService {[DebuggerStepThrough] get; set; }

        [Inject]
        public IByteSizeCalculating ByteSizeCalculatingService { get; set; }

        [Inject]
        public IBookTagInitializing BookTagInitializingService { get; set; }

        [Inject]
        public IEncryptionStarting EncryptionStartingService { get; set; }

        [Inject]
        public IUnencryptionStarting UnencryptionStartingService { get; set; }

        [Inject]
        public IDataAccessManager DataAccessManager {[DebuggerStepThrough] get; set; }

        #endregion //プロパティ

        public async Task Reset()
        {
            await TaskManager.Enqueue(LibraryResettingService.GetTaskSequence());
        }

        #region 初期化&ロード

        public async Task Initialize()
        {
            await TaskManager.Enqueue(LibraryInitializingService.GetTaskSequence());
        }

        public async Task Load()
        {
            try
            {
                await TaskManager.Enqueue(LibraryLoadingService.GetTaskSequence());
            }
            catch (FailedOpeningDatabaseException e)
            {
                s_logger.Fatal("Fail to connect database．Sunctum will be terminated．", e);
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
                await TaskManager.Enqueue(BookImportingService.GetTaskSequence());
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
                await TaskManager.Enqueue(LibraryImportingService.GetTaskSequence());
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
            await TaskManager.Enqueue(BookRemovingService.GetTaskSequence());
        }

        public async Task RemovePages(PageViewModel[] pages)
        {
            PageRemovingService.TargetPages = pages;
            await TaskManager.Enqueue(PageRemovingService.GetTaskSequence());
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

        [Inject]
        public IPageOrderUpdating PageOrderUpdatingService { get; set; }

        public async Task SaveBookContentsOrder(BookViewModel target)
        {
            PageOrderUpdatingService.Target = target;
            await TaskManager.Enqueue(PageOrderUpdatingService.GetTaskSequence());
        }

        #endregion //ソート

        #region スクラップ

        public async Task ScrapPages(AuthorViewModel author, string title, PageViewModel[] pages)
        {
            PageScrappingService.Title = title;
            PageScrappingService.TargetPages = pages;
            PageScrappingService.MasterDirectory = Specifications.MASTER_DIRECTORY;
            await TaskManager.Enqueue(PageScrappingService.GetTaskSequence());
        }

        #endregion //スクラップ

        #region エクスポート

        public async Task ExportBooks(BookViewModel[] books, string directory, bool tag)
        {
            BookExportingService.TargetBooks = books;
            BookExportingService.DestinationDirectory = directory;
            BookExportingService.IncludeTag = tag;
            await TaskManager.Enqueue(BookExportingService.GetTaskSequence());
        }

        #endregion //エクスポート

        #region サムネイル再作成

        public async Task RemakeThumbnail(IEnumerable<BookViewModel> books)
        {
            BookThumbnailRemakingService.TargetBooks = books;
            await TaskManager.Enqueue(BookThumbnailRemakingService.GetTaskSequence());
        }

        public async Task RemakeThumbnail(IEnumerable<PageViewModel> pages)
        {
            PageThumbnailRemakingService.TargetPages = pages;
            await TaskManager.Enqueue(PageThumbnailRemakingService.GetTaskSequence());
        }

        #endregion //サムネイル再作成

        #region 問い合わせ

        public bool IsDirty(BookViewModel book)
        {
            return Querying.IsDirty(this, book);
        }

        #endregion //問い合わせ

        #region サイズ更新

        public async Task UpdateBookByteSizeAll()
        {
            ByteSizeCalculatingService.LibraryManager = this;
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsAll;
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence());
        }

        public async Task UpdateBookByteSizeStillNull()
        {
            ByteSizeCalculatingService.LibraryManager = this;
            ByteSizeCalculatingService.Range = ByteSizeCalculating.UpdateRange.IsStillNull;
            await TaskManager.Enqueue(ByteSizeCalculatingService.GetTaskSequence());
        }

        #endregion //サイズ更新

        public async Task UpdateBookTag()
        {
            await TaskManager.Enqueue(BookTagInitializingService.GetTaskSequence());
        }

        public IArrangedBookStorage CreateBookStorage()
        {
            return new BookCabinet(this.BookSource);
        }

        public async Task StartEncryption(string password)
        {
            EncryptionStartingService.LibraryManager = this;
            EncryptionStartingService.Password = password;
            await TaskManager.Enqueue(EncryptionStartingService.GetTaskSequence());
        }

        public bool UnlockIfLocked()
        {
            var encryptedItems = EncryptImageFacade.FindAll();
            if (encryptedItems.Count() == 0)
            {
                return false;
            }

            InputPasswordDialog dialog = new InputPasswordDialog();

            if (dialog.ShowDialog() == true)
            {
                Configuration.ApplicationConfiguration.Password = dialog.Password;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task StartUnencryption(string password)
        {
            UnencryptionStartingService.LibraryManager = this;
            UnencryptionStartingService.Password = password;
            await TaskManager.Enqueue(UnencryptionStartingService.GetTaskSequence());
        }
    }
}
