

using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Converters;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public abstract class DocumentViewModelBase : DockElementViewModelBase, IDocumentViewModelBase
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private IArrangedBookStorage _Cabinet;
        private Dictionary<Guid, Point> _scrollOffset;
        private List<EntryViewModel> _SelectedEntries;
        private ObservableCollection<BookViewModel> _BookListViewSelectedItems;
        private List<PageViewModel> _ContentsListViewSelectedItems;
        private bool _SearchPaneIsVisible;
        private BookViewModel _OpenedBook;
        private PageViewModel _OpenedPage;
        private string _SearchText;
        private ObservableCollection<Control> _BooksContextMenuItems;
        private ObservableCollection<Control> _ContentsContextMenuItems;
        private string _previousSearchingText;
        private bool _BookListIsVisible;
        private bool _ContentListIsVisible;
        private bool _ImageIsVisible;

        public static readonly Guid BeforeSearchPosition = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

        public ILibrary LibraryManager { get; set; }

        public IMainWindowViewModel MainWindowViewModel { get; set; }

        #region コマンド

        public ICommand BuildBookContextMenuCommand { get; set; }

        public ICommand BuildContentsContextMenuCommand { get; set; }

        public ICommand ChangeStarCommand { get; set; }

        public ICommand CloseSearchPaneCommand { get; set; }

        public ICommand CloseTabCommand { get; set; }

        public ICommand ExportBooksCommand { get; set; }

        public ICommand FilterBooksCommand { get; set; }

        public ICommand LeftKeyDownCommand { get; set; }

        public ICommand MouseWheelCommand { get; set; }

        public ICommand OpenBookCommand { get; set; }

        public ICommand OpenBookInNewTabCommand { get; set; }

        public ICommand OpenBookPropertyDialogCommand { get; set; }

        public ICommand OpenImageByDefaultProgramCommand { get; set; }

        public ICommand OpenSearchPaneCommand { get; set; }

        public ICommand RemakeThumbnailOfBookCommand { get; set; }

        public ICommand RemakeThumbnailOfPageCommand { get; set; }

        public ICommand RemoveBookCommand { get; set; }

        public ICommand RemovePageCommand { get; set; }

        public ICommand RightKeyDownCommand { get; set; }

        public ICommand SearchInNewTabCommand { get; set; }

        public ICommand SendBookToExistTabCommand { get; set; }

        public ICommand SendBookToNewTabCommand { get; set; }

        public ICommand ScrapPagesCommand { get; set; }

        public ICommand XButton1MouseButtonDownCommand { get; set; }

        public ICommand XButton2MouseButtonDownCommand { get; set; }

        #endregion //コマンド

        #region プロパティ

        public InteractionRequest<Notification> ChangeStarRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> ResetScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> BlinkGoNextButtonRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> BlinkGoBackButtonRequest { get; } = new InteractionRequest<Notification>();

        public IArrangedBookStorage BookCabinet
        {
            get { return _Cabinet; }
            set { SetProperty(ref _Cabinet, value); }
        }

        public List<EntryViewModel> SelectedEntries
        {
            [DebuggerStepThrough]
            get
            { return _SelectedEntries; }
            protected set { SetProperty(ref _SelectedEntries, value); }
        }

        public ObservableCollection<BookViewModel> BookListViewSelectedItems
        {
            get { return _BookListViewSelectedItems; }
            set { SetProperty(ref _BookListViewSelectedItems, value); }
        }

        public List<PageViewModel> ContentsListViewSelectedItems
        {
            get { return _ContentsListViewSelectedItems; }
            set { SetProperty(ref _ContentsListViewSelectedItems, value); }
        }

        public override bool CanClose => false;

        public bool SearchPaneIsVisible
        {
            get { return _SearchPaneIsVisible; }
            set { SetProperty(ref _SearchPaneIsVisible, value); }
        }

        public BookViewModel OpenedBook
        {
            [DebuggerStepThrough]
            get
            { return _OpenedBook; }
            set { SetProperty(ref _OpenedBook, value); }
        }

        public PageViewModel OpenedPage
        {
            [DebuggerStepThrough]
            get
            { return _OpenedPage; }
            set
            {
                SetProperty(ref _OpenedPage, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => CurrentPage));
            }
        }

        public int CurrentPage
        {
            get
            {
                if (OpenedBook != null && OpenedPage != null)
                {
                    return OpenedBook.Contents.IndexOf(OpenedPage) + 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public string SearchText
        {
            [DebuggerStepThrough]
            get
            { return _SearchText; }
            set
            {
                SetProperty(ref _SearchText, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => SearchStatusText));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => UnescapedSearchText));
            }
        }

        public string UnescapedSearchText
        {
            [DebuggerStepThrough]
            get
            { return HttpUtility.HtmlDecode(SearchText); }
            set { SearchText = HttpUtility.HtmlEncode(value); }
        }

        public string SearchStatusText
        {
            get { return $"Searched by '{UnescapedSearchText}'"; }
        }

        public ObservableCollection<Control> BooksContextMenuItems
        {
            get { return _BooksContextMenuItems; }
            set { SetProperty(ref _BooksContextMenuItems, value); }
        }

        public ObservableCollection<Control> ContentsContextMenuItems
        {
            get { return _ContentsContextMenuItems; }
            set { SetProperty(ref _ContentsContextMenuItems, value); }
        }

        public bool BookListIsVisible
        {
            get { return _BookListIsVisible; }
            set { SetProperty(ref _BookListIsVisible, value); }
        }

        public bool ContentListIsVisible
        {
            get { return _ContentListIsVisible; }
            set { SetProperty(ref _ContentListIsVisible, value); }
        }

        public bool ImageIsVisible
        {
            get { return _ImageIsVisible; }
            set { SetProperty(ref _ImageIsVisible, value); }
        }

        public ReactiveProperty<int?> StarLevel { get; } = new ReactiveProperty<int?>();

        #endregion //プロパティ

        public DocumentViewModelBase()
        {
            RegisterCommands();
            SelectedEntries = new List<EntryViewModel>();
            BookListIsVisible = true;
            ContentListIsVisible = false;
            ImageIsVisible = false;
            BookListViewSelectedItems = new ObservableCollection<BookViewModel>();
            BookListViewSelectedItems.CollectionChangedAsObservable()
                .Subscribe(x =>
                {
                    UpdateStarLevel();
                });
        }

        private void UpdateStarLevel()
        {
            StarLevel.Value = BookListViewSelectedItems.Any() ? BookListViewSelectedItems.First().StarLevel : null;
        }

        private void RegisterCommands()
        {
            BuildBookContextMenuCommand = new DelegateCommand<FrameworkElement>(fe =>
            {
                BuildContextMenus_Books();
                fe.ContextMenu.IsOpen = true;
            });
            BuildContentsContextMenuCommand = new DelegateCommand<FrameworkElement>(fe =>
            {
                BuildContextMenus_Contents();
                fe.ContextMenu.IsOpen = true;
            });
            ChangeStarCommand = new DelegateCommand(() =>
            {
                ChangeStarRequest.Raise(new Notification() { Title = "Change Star", Content = BookListViewSelectedItems.First() });
                UpdateStarLevel();
            });
            CloseTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.CloseTab(this);
            });
            CloseSearchPaneCommand = new DelegateCommand(() =>
            {
                CloseSearchPane();
            });
            ExportBooksCommand = new DelegateCommand(() =>
            {
                var books = BookListViewSelectedItems;
                OpenExportDialog(books.ToArray());
            });
            FilterBooksCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.NewContentTab(BookListViewSelectedItems);
            });
            LeftKeyDownCommand = new DelegateCommand(() =>
            {
                if (OpenedPage != null)
                {
                    GoPreviousImage();
                    BeginAnimation_Tick_PreviousImageButton();
                }
                else if (OpenedBook != null)
                {
                    //Do nothing
                }
                else
                {
                    //Do nothing
                }
            });
            MouseWheelCommand = new DelegateCommand<int?>(delta =>
            {
                if (OpenedPage != null)
                {
                    if (delta.Value > 0) //奥方向に回転
                    {
                        GoPreviousImage();
                    }
                    else if (delta.Value < 0) //手前方向に回転
                    {
                        GoNextImage();
                    }
                }
                else if (OpenedBook != null)
                {
                    //Do nothing
                }
                else
                {
                    //Do nothing
                }
            });
            OpenBookCommand = new DelegateCommand(() =>
            {
                OpenBook(BookListViewSelectedItems.First());
            });
            OpenBookInNewTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.NewContentTab(BookListViewSelectedItems.First());
                MainWindowViewModel.ActiveDocumentViewModel.OpenBook(BookListViewSelectedItems.First());
            });
            OpenBookPropertyDialogCommand = new DelegateCommand(() =>
            {
                var books = BookListViewSelectedItems;
                OpenBookPropertyDialog(books.First());
            });
            OpenImageByDefaultProgramCommand = new DelegateCommand<object>((p) =>
            {
                OpenImageByDefaultProgram(p as IEnumerable<PageViewModel>);
            });
            OpenSearchPaneCommand = new DelegateCommand(() =>
            {
                OpenSearchPane();
            });
            RightKeyDownCommand = new DelegateCommand(() =>
            {
                if (OpenedPage != null)
                {
                    GoNextImage();
                    BeginAnimation_Tick_NextImageButton();
                }
                else if (OpenedBook != null)
                {
                    //Do nothing
                }
                else
                {
                    //Do nothing
                }
            });
            RemakeThumbnailOfBookCommand = new DelegateCommand(async () =>
            {
                var books = BookListViewSelectedItems;
                await RemakeThumbnail(books);
            });
            RemakeThumbnailOfPageCommand = new DelegateCommand(async () =>
            {
                var pages = ContentsListViewSelectedItems;
                await RemakeThumbnail(pages);
            });
            RemoveBookCommand = new DelegateCommand(async () =>
            {
                var books = BookListViewSelectedItems;
                await RemoveBook(books.ToArray());
            });
            RemovePageCommand = new DelegateCommand<object>(async (p) =>
            {
                await RemovePage(p as IEnumerable<PageViewModel>);
            });
            SearchInNewTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.NewSearchTab(BookCabinet.OnStage);
            });
            SendBookToExistTabCommand = new DelegateCommand<IDocumentViewModelBase>(p =>
            {
                foreach (var item in BookListViewSelectedItems)
                {
                    p.BookCabinet.AddToMemory(item);
                }
            });
            SendBookToNewTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.NewContentTab(BookListViewSelectedItems);
            });
            ScrapPagesCommand = new DelegateCommand<object>(async (p) =>
            {
                await ScrapPages(p as IEnumerable<PageViewModel>);
            });
            XButton1MouseButtonDownCommand = new DelegateCommand(() =>
            {
                if (OpenedPage != null)
                {
                    CloseImage();
                }
                else if (OpenedBook != null)
                {
                    CloseBook();
                }
                else
                {
                    if (BookCabinet.IsSearching)
                    {
                        ClearSearchResult();
                        CloseSearchPane();
                        RestoreScrollOffset(BeforeSearchPosition);
                    }
                }
            });
            XButton2MouseButtonDownCommand = new DelegateCommand(() =>
            {
                if (OpenedPage != null)
                {
                    //Do nothing
                }
                else if (OpenedBook != null)
                {
                    //Do nothing
                }
                else
                {
                    //Do nothing
                }
            });
        }

        public void ResetScrollOffsetPool()
        {
            _scrollOffset = new Dictionary<Guid, Point>();
        }

        public void StoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty) || bookId.Equals(BeforeSearchPosition))
            {
                if (_scrollOffset.ContainsKey(bookId)) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    StoreBookScrollOffsetRequest.Raise(new Notification()
                    {
                        Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                    });
                });
            }
            else
            {
                StoreContentScrollOffsetRequest.Raise(new Notification()
                {
                    Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                });
            }
        }

        public void RestoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty) || bookId.Equals(BeforeSearchPosition))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RestoreBookScrollOffsetRequest.Raise(new Notification()
                    {
                        Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                    });
                });
            }
            else
            {
                RestoreContentScrollOffsetRequest.Raise(new Notification()
                {
                    Content = new Tuple<Dictionary<Guid, Point>, Guid>(_scrollOffset, bookId)
                });
            }

            _scrollOffset.Remove(bookId);
        }

        public void ResetScrollOffset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ResetScrollOffsetRequest.Raise(new Notification());
            });
        }

        #region 操作

        public void AddToSelectedEntry(EntryViewModel add)
        {
            SelectedEntries.Add(add);
        }

        public void AddToSelectedEntries(IEnumerable<EntryViewModel> add)
        {
            SelectedEntries.AddRange(add);
        }

        public void RemoveFromSelectedEntries(IEnumerable<EntryViewModel> entries)
        {
            foreach (var entry in entries)
            {
                SelectedEntries.Remove(entry);
            }
        }

        #endregion //操作

        #region 初期化

        public void ClearSelectedItems()
        {
            BookListViewSelectedItems.Clear();
            ContentsListViewSelectedItems = new List<PageViewModel>();
        }

        #endregion //初期化

        #region コンテキストメニュー

        public void BuildContextMenus_Books()
        {
            var menulist = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new MenuItem()
            {
                Header = "開く",
                Command = OpenBookCommand
            };
            menulist.Add(menuitem);

            menuitem = new MenuItem()
            {
                Header = "新しいタブで開く",
                Command = OpenBookInNewTabCommand
            };
            menulist.Add(menuitem);

            menuitem = new MenuItem()
            {
                Header = "選択したアイテムで絞り込む",
                Command = FilterBooksCommand
            };
            menulist.Add(menuitem);

            menuitem = new MenuItem()
            {
                Header = "送る"
            };
            menuitem.Items.Add(new MenuItem()
            {
                Header = "新しいタブ",
                Command = SendBookToNewTabCommand
            });
            foreach (var item in MainWindowViewModel.TabItemViewModels.Where(t => !t.ContentId.Equals("home") && !t.ContentId.Equals(this.ContentId)))
            {
                menuitem.Items.Add(new MenuItem()
                {
                    Header = item.Title,
                    Command = SendBookToExistTabCommand,
                    CommandParameter = item
                });
            }
            menulist.Add(menuitem);

            menuitem = new MenuItem()
            {
                Header = "DUMMY",
                Command = ChangeStarCommand
            };
            var binding = new Binding("StarLevel.Value") { Converter = new StarLevelToStringConverter(), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            menuitem.SetBinding(MenuItem.HeaderProperty, binding);
            menulist.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "書き出し",
                Command = ExportBooksCommand
            };
            menulist.Add(menuitem);

            var manageMenu = new System.Windows.Controls.MenuItem()
            {
                Header = "管理"
            };

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "サムネイル再作成",
                Command = RemakeThumbnailOfBookCommand
            };
            manageMenu.Items.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "削除",
                Command = RemoveBookCommand
            };
            manageMenu.Items.Add(menuitem);
            menulist.Add(manageMenu);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "プロパティ",
                Command = OpenBookPropertyDialogCommand,
            };
            menulist.Add(menuitem);

            menulist.Add(new System.Windows.Controls.Separator());

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Extra",
            };
            binding = new Binding("MainWindowViewModel.ExtraBookContextMenu");
            menuitem.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            menulist.Add(menuitem);

            BooksContextMenuItems = menulist;
        }

        public void BuildContextMenus_Contents()
        {
            var menulist = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "開く",
                Command = OpenImageByDefaultProgramCommand,
                CommandParameter = SelectedEntries.Where(x => x is PageViewModel).Cast<PageViewModel>()
            };
            menulist.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "スクラップ",
                Command = ScrapPagesCommand,
                CommandParameter = SelectedEntries.Where(x => x is PageViewModel).Cast<PageViewModel>()
            };
            menulist.Add(menuitem);

            var manageMenu = new System.Windows.Controls.MenuItem()
            {
                Header = "管理"
            };

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "サムネイル再作成",
                Command = RemakeThumbnailOfPageCommand
            };
            manageMenu.Items.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "削除",
                Command = RemovePageCommand,
                CommandParameter = SelectedEntries.Where(x => x is PageViewModel).Cast<PageViewModel>()
            };
            manageMenu.Items.Add(menuitem);
            menulist.Add(manageMenu);

            menulist.Add(new System.Windows.Controls.Separator());

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Extra",
            };
            var binding = new Binding("MainWindowViewModel.ExtraPageContextMenu");
            menuitem.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            menulist.Add(menuitem);

            ContentsContextMenuItems = menulist;
        }

        private void OpenBookPropertyDialog(BookViewModel book)
        {
            var dialog = new BookPropertyDialog(book);
            dialog.ShowDialog();
        }

        private void OpenExportDialog(BookViewModel[] books)
        {
            var dialog = new ExportDialog(LibraryManager, books);
            dialog.ShowDialog();
        }

        private static void OpenImageByDefaultProgram(IEnumerable<PageViewModel> images)
        {
            Process.Start(images.First().Image.AbsoluteMasterPath);
        }

        public async Task RemovePage(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.RemovePages(pages.ToArray());
        }

        public async Task RemoveBook(BookViewModel[] books)
        {
            await LibraryManager.RemoveBooks(books);
        }

        private async Task ScrapPages(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.ScrapPages(null, Specifications.SCRAPPED_NEW_BOOK_TITLE, pages.ToArray());
        }

        #endregion //コンテキストメニュー

        #region 検索

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
                    BookCabinet.SearchedBooks = null;

                    OnSearchCleared(new EventArgs());
                }
                else
                {
                    s_logger.Debug($"Search Word:{searchingText}");
                    BookCabinet.SearchedBooks = new ObservableCollection<BookViewModel>(BookCabinet.BookSource.Where(b => AuthorNameContainsSearchText(b, searchingText) || TitleContainsSearchText(b, searchingText)));

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

        public void Search()
        {
            Search(SearchText);
        }

        public void ClearSearchResult()
        {
            SearchText = "";
            if (BookCabinet != null)
            {
                BookCabinet.ClearSearchResult();
            }
        }

        public void OpenSearchPane()
        {
            SearchPaneIsVisible = true;
        }

        public void CloseSearchPane()
        {
            SearchPaneIsVisible = false;
        }

        #endregion //検索

        #region サムネイル再作成

        private async Task RemakeThumbnail(IEnumerable<BookViewModel> books)
        {
            await LibraryManager.RemakeThumbnail(books);
        }

        private async Task RemakeThumbnail(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.RemakeThumbnail(pages);
        }

        #endregion //サムネイル再作成

        #region 画面遷移

        public void OpenBook(BookViewModel book)
        {
            if (book == null) return;

            CloseSearchPane();
            StoreScrollOffset(Guid.Empty);
            OpenedBook = book;
            RestoreScrollOffset(OpenedBook.ID);
            this.MainWindowViewModel.LibraryVM.TagMng.ClearSelectedEntries();
            Task.Factory.StartNew(() => this.MainWindowViewModel.LibraryVM.FireFillContents(book));
            BookListIsVisible = false;
            ContentListIsVisible = true;
        }

        public void CloseBook()
        {
            BookListIsVisible = true;
            ContentListIsVisible = false;
            if (OpenedBook != null)
            {
                StoreScrollOffset(OpenedBook.ID);
                RestoreScrollOffset(Guid.Empty);
                this.MainWindowViewModel.LibraryVM.TagMng.ClearSelectedEntries();
            }
            OpenedBook = null;
        }

        public void OpenImage(PageViewModel page)
        {
            OpenedPage = page;
            ContentListIsVisible = false;
            ImageIsVisible = true;
        }

        public void CloseImage()
        {
            OpenedPage = null;
            ContentListIsVisible = true;
            ImageIsVisible = false;
        }

        #endregion

        #region Image遷移

        public void GoNextImage()
        {
            var currentPage = OpenedPage;

            OpenedPage = OpenedBook.Contents.LoopNext(currentPage);

            ContentsLoadTask.Load(OpenedPage);

            if (OpenedPage.IsLoaded)
            {
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => CurrentPage));
            }
        }

        public void GoPreviousImage()
        {
            var currentPage = OpenedPage;

            OpenedPage = OpenedBook.Contents.LoopPrevious(currentPage);

            ContentsLoadTask.Load(OpenedPage);

            if (OpenedPage.IsLoaded)
            {
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => CurrentPage));
            }
        }

        private PageViewModel GoFirstImage()
        {
            return OpenedBook.Contents.OrderBy(p => p.PageIndex).Take(1).Single();
        }

        private PageViewModel GoLastImage()
        {
            return OpenedBook.Contents.OrderByDescending(p => p.PageIndex).Take(1).Single();
        }

        #endregion //Image遷移

        #region ページ単位ソート

        public void MovePageForward(PageViewModel page)
        {
            OpenedBook = LibraryManager.OrderForward(page, OpenedBook);
        }

        public void MovePageBackward(PageViewModel page)
        {
            OpenedBook = LibraryManager.OrderBackward(page, OpenedBook);
        }

        public async Task SaveOpenedBookContentsOrder()
        {
            await LibraryManager.SaveBookContentsOrder(OpenedBook);
            SetFirstPage();
        }

        private void SetFirstPage()
        {
            if (OpenedBook.Contents.Count() > 0)
            {
                OpenedBook.FirstPage = OpenedBook.Contents.First();
            }
        }

        #endregion //ページ単位ソート

        #region キーボード操作

        private void BeginAnimation_Tick_PreviousImageButton()
        {
            BlinkGoBackButtonRequest.Raise(new Notification());
        }

        private void BeginAnimation_Tick_NextImageButton()
        {
            BlinkGoNextButtonRequest.Raise(new Notification());
        }

        #endregion //キーボード操作

        #region インポート

        public async Task ImportAsync(string[] objectPaths)
        {
            await LibraryManager.ImportAsync(objectPaths);
        }

        #endregion //インポート

        #region 問い合わせ

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(this.BookCabinet.Sorting, name);
        }

        #endregion //問い合わせ
    }
}
