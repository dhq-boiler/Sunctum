

using Ninject;
using NLog;
using Prism.Mvvm;
using Sunctum.Domail.Util;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Logic.PageSorting;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sunctum.Managers
{
    public class LibraryManager : BindableBase, ILibraryManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<BookViewModel> _LoadedBooks;
        private ObservableCollection<BookViewModel> _SearchedBooks;
        private IBookSorting _BookSorting;
        private FillContentsTaskManager _fcTaskManager = new FillContentsTaskManager();

        public LibraryManager()
        {
            Sorting = BookSorting.ByLoadedAsc;
            LoadedBooks = new ObservableCollection<BookViewModel>();
        }

        #region プロパティ

        public ObservableCollection<BookViewModel> LoadedBooks
        {
            [DebuggerStepThrough]
            get
            { return _LoadedBooks; }
            set
            {
                SetProperty(ref _LoadedBooks, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public ObservableCollection<BookViewModel> SearchedBooks
        {
            [DebuggerStepThrough]
            get
            { return _SearchedBooks; }
            set
            {
                SetProperty(ref _SearchedBooks, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => IsSearching));
            }
        }

        private ObservableCollection<BookViewModel> DisplayableBookSource
        {
            [DebuggerStepThrough]
            get
            {
                if (SearchedBooks != null)
                {
                    return SearchedBooks;
                }
                else
                {
                    return LoadedBooks;
                }
            }
        }

        public ObservableCollection<BookViewModel> OnStage
        {
            get
            {
                var newCollection = Sorting.Sort(DisplayableBookSource).ToArray();
                return new ObservableCollection<BookViewModel>(newCollection);
            }
        }

        public bool IsSearching
        {
            [DebuggerStepThrough]
            get
            { return SearchedBooks != null; }
        }

        public IBookSorting Sorting
        {
            [DebuggerStepThrough]
            get
            { return _BookSorting; }
            set
            {
                SetProperty(ref _BookSorting, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

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

        #region オンメモリ

        public void AddToMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_AddToMemory(book));
        }

        public void UpdateInMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_UpdateInMemory(book));
        }

        public void RemoveFromMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_RemoveBookFromMemory(book));
        }

        #region private

        private void Internal_AddToMemory(BookViewModel book)
        {
            LoadedBooks.Add(book);
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        private void Internal_UpdateInMemory(BookViewModel book)
        {
            BookFacade.Update(book);
            int index = LoadedBooks.IndexOf(LoadedBooks.Where(b => b.ID.Equals(book.ID)).Single());
            LoadedBooks[index] = book;
            if (SearchedBooks != null)
            {
                index = SearchedBooks.IndexOf(SearchedBooks.Where(b => b.ID.Equals(book.ID)).Single());
                SearchedBooks[index] = book;
            }
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        private void Internal_RemoveBookFromMemory(BookViewModel book)
        {
            LoadedBooks.Remove(book);
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        #endregion //private

        #endregion //オンメモリ

        #endregion //エンティティ操作

        #region 検索

        private string _previousSearchingText;
        private ObservableCollection<RecentOpenedLibrary> _RecentOpenedLibraryList;

        public event EventHandler SearchCleared;
        public event SearchedEventHandler Searched;

        protected virtual void OnSearchCleared(EventArgs e)
        {
            SearchCleared?.Invoke(this, e);
        }

        protected virtual void OnSearched(SearchedEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        public void Search(string searchingText)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(searchingText))
                {
                    SearchedBooks = null;

                    OnSearchCleared(new EventArgs());
                }
                else
                {
                    s_logger.Debug($"Search Word:{searchingText}");
                    SearchedBooks = new ObservableCollection<BookViewModel>(LoadedBooks.Where(b => AuthorNameContainsSearchText(b, searchingText) || TitleContainsSearchText(b, searchingText)));

                    OnSearched(new SearchedEventArgs(searchingText, _previousSearchingText));
                }

                _previousSearchingText = searchingText;
            });
        }

        private bool AuthorNameContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null || target.Author == null)
            {
                return false;
            }
            return target.Author.Name.IndexOf(searchingText) != -1;
        }

        private bool TitleContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null)
            {
                return false;
            }
            return target.Title.IndexOf(searchingText) != -1;
        }

        public void ClearSearchResult()
        {
            SearchedBooks = null;

            TagMng?.ClearSearchResult();
            AuthorManager?.ClearSearchResult();
        }

        #endregion //検索

        #region コンテンツ読み込み

        public void FireFillContents(BookViewModel book)
        {
            _fcTaskManager.RunAsync((b) => Internal_FillContents(b), book);
        }

        public void RunFillContents(BookViewModel book)
        {
            _fcTaskManager.Run((b) => Internal_FillContents(b), book);
        }

        public void FireFillContentsWithImage(BookViewModel book)
        {
            _fcTaskManager.RunAsync((b) => Internal_FillContentsWithImage(b), book);
        }

        public void RunFillContentsWithImage(BookViewModel book)
        {
            _fcTaskManager.Run((b) => Internal_FillContentsWithImage(b), book);
        }

        private void Internal_FillContents(BookViewModel book)
        {
            int currentCount = Querying.BookContentsCount(book.ID);
            if (currentCount != book.Contents.Count() || !book.Contents.All(b => b.IsLoaded))
            {
                ContentsLoadTask.FillContents(this, book);
            }
        }

        private void Internal_FillContentsWithImage(BookViewModel book)
        {
            int currentCount = Querying.BookContentsCount(book.ID);
            if (currentCount != book.Contents.Count() || !book.Contents.All(b => b.IsLoaded))
            {
                ContentsLoadTask.FillContentsWithImage(this, book);
            }
        }

        #endregion //コンテンツ読み込み

        #region 一般

        public void RunTasks(List<Task> tasks, bool progressEnables = true)
        {
            int total = tasks.Count;

            var timekeeper = new TimeKeeper();

            for (int i = 0; i < total; ++i)
            {
                if (progressEnables)
                {
                    ProgressManager.UpdateProgress(i, total, timekeeper);
                }

                var t = tasks[i];

                t.RunSynchronously();

                if (total < tasks.Count)
                {
                    total = tasks.Count;
                }
            }

            if (progressEnables)
            {
                ProgressManager.Complete();
            }
        }

        #endregion //一般

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

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(this.Sorting, name);
        }

        #endregion //問い合わせ

        #region サイズ更新

        [Inject]
        public IByteSizeCalculating ByteSizeCalculatingService { get; set; }

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

        #region DispatcherObjectアクセス

        public void AccessDispatcherObject(Action accessAction)
        {
            if (Application.Current?.Dispatcher == null) //For UnitTest
            {
                accessAction.Invoke();
                return;
            }

            try
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    accessAction.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(accessAction);
                }
            }
            catch (NullReferenceException)
            { }
        }

        #endregion
    }
}
