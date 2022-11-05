

using Homura.Core;
using NLog;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Converters;
using Sunctum.Core.Extensions;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Logic.DisplayType;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Managers;
using Sunctum.Plugin;
using Sunctum.UI.Controls;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unity;

namespace Sunctum.ViewModels
{
    public abstract class DocumentViewModelBase : DockElementViewModelBase, IDocumentViewModelBase, IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        protected CompositeDisposable disposables = new CompositeDisposable();
        private IArrangedBookStorage _Cabinet;
        private Dictionary<Guid, Point> _scrollOffset;
        private List<EntryViewModel> _SelectedEntries;
        private ObservableCollection<BookViewModel> _BookListViewSelectedItems;
        private List<PageViewModel> _ContentsListViewSelectedItems;
        private bool _SearchPaneIsVisible;
        private BookViewModel _OpenedBook;
        private PageViewModel _OpenedPage;
        private ObservableCollection<Control> _BooksContextMenuItems;
        private ObservableCollection<Control> _ContentsContextMenuItems;
        private string _previousSearchingText;
        private bool _BookListIsVisible;
        private bool _ContentListIsVisible;
        private bool _ImageIsVisible;
        private bool disposedValue;
        public static readonly Guid BeforeSearchPosition = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        private readonly IDialogService dialogService;

        [Dependency]
        public ISelectManager SelectManager { get; set; }

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }

        #region コマンド

        public ICommand BlinkGoBackButtonCommand { get; set; }

        public ICommand BlinkGoNextButtonCommand { get; set; }

        public ICommand BuildBookContextMenuCommand { get; set; }

        public ICommand BuildContentsContextMenuCommand { get; set; }

        public ICommand ChangeStarCommand { get; set; }

        public ICommand CloseSearchPaneCommand { get; set; }

        public ICommand CloseTabCommand { get; set; }

        public ReactiveCommand<DragEventArgs> DropCommand { get; set; } = new ReactiveCommand<DragEventArgs>();

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

        [Dependency]
        public IEnumerable<Lazy<IDropPlugin>> DropPlugins { get; set; }

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

        public DocumentViewModelBase(IDialogService dialogService)
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
            this.dialogService = dialogService;
        }

        private void UpdateStarLevel()
        {
            StarLevel.Value = BookListViewSelectedItems.Any() ? BookListViewSelectedItems.First().StarLevel : null;
        }

        private void RegisterCommands()
        {
            BlinkGoBackButtonCommand = new DelegateCommand(() =>
            {
                GoPreviousImage();
            });
            BlinkGoNextButtonCommand = new DelegateCommand(() =>
            {
                GoNextImage();
            });
            BuildBookContextMenuCommand = new DelegateCommand<ContextMenuEventArgs>(args =>
            {
                BuildContextMenus_Books();
                (args.Source as FrameworkElement).ContextMenu.IsOpen = true;
            });
            BuildContentsContextMenuCommand = new DelegateCommand<ContextMenuEventArgs>(args =>
            {
                BuildContextMenus_Contents();
                (args.Source as FrameworkElement).ContextMenu.IsOpen = true;
            });
            ChangeStarCommand = new DelegateCommand<ObservableCollection<BookViewModel>>(args =>
            {
                IDialogResult result = new DialogResult();
                IDialogParameters parameters = new DialogParameters();
                parameters.Add("Book", args.First());
                dialogService.ShowDialog(nameof(ChangeStar), parameters, ret => result = ret);
                UpdateStarLevel();
            });
            CloseTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.Value.CloseTab(this);
            });
            CloseSearchPaneCommand = new DelegateCommand(() =>
            {
                CloseSearchPane();
            });
            DropCommand.Subscribe(args =>
            {
                foreach (var dropPlugin in DropPlugins)
                {
                    dropPlugin.Value.Execute(args.Data);
                }
            })
            .AddTo(disposables);
            ExportBooksCommand = new DelegateCommand(() =>
            {
                var books = BookListViewSelectedItems;
                OpenExportDialog(books.ToArray());
            });
            FilterBooksCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.Value.NewContentTab(BookListViewSelectedItems);
            });
            LeftKeyDownCommand = new DelegateCommand(() =>
            {
                if (OpenedPage != null)
                {
                    GoPreviousImage();
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
            MouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(args =>
            {
                var delta = args.Delta;

                if (OpenedPage != null)
                {
                    if (delta > 0) //奥方向に回転
                    {
                        GoPreviousImage();
                    }
                    else if (delta < 0) //手前方向に回転
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
                MainWindowViewModel.Value.NewContentTab(BookListViewSelectedItems.First());
                MainWindowViewModel.Value.ActiveDocumentViewModel.OpenBook(BookListViewSelectedItems.First());
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
                await RemakeThumbnail(books).ConfigureAwait(false);
            });
            RemakeThumbnailOfPageCommand = new DelegateCommand(async () =>
            {
                var pages = ContentsListViewSelectedItems;
                await RemakeThumbnail(pages).ConfigureAwait(false);
            });
            RemoveBookCommand = new DelegateCommand(async () =>
            {
                var books = BookListViewSelectedItems;
                await RemoveBook(books.ToArray()).ConfigureAwait(false);
            });
            RemovePageCommand = new DelegateCommand<object>(async (p) =>
            {
                await RemovePage(p as IEnumerable<PageViewModel>).ConfigureAwait(false);
            });
            SearchInNewTabCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.Value.NewSearchTab(BookCabinet.OnStage);
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
                MainWindowViewModel.Value.NewContentTab(BookListViewSelectedItems);
            });
            ScrapPagesCommand = new DelegateCommand<object>(async (p) =>
            {
                await ScrapPages(p as IEnumerable<PageViewModel>).ConfigureAwait(false);
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
                        BookCabinet.ClearSearchResult();
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
                    var displayType = BookCabinet.DisplayType;
                    if (displayType == DisplayType.SideBySide)
                    {
                        var virtualizingWrapPanel = Application.Current.MainWindow.FindChild<VirtualizingWrapPanel>("BookListViewVirtualinzingWrapPanel");
                        _scrollOffset[bookId] = virtualizingWrapPanel.GetOffset();
                    }
                    else if (displayType == DisplayType.Details)
                    {
                        var virtualizingStackPanel = Application.Current.MainWindow.FindChild<System.Windows.Controls.VirtualizingStackPanel>("BookListViewDetailsVirtualizingStackPanel");
                        _scrollOffset[bookId] = new Point(virtualizingStackPanel.HorizontalOffset, virtualizingStackPanel.VerticalOffset);
                    }
                });
            }
            else
            {
                var virtualizingWrapPanel = Application.Current.MainWindow.FindChild<VirtualizingWrapPanel>("ContentsListViewVirtualizingWrapPanel");
                if (virtualizingWrapPanel is null) return;
                _scrollOffset[bookId] = virtualizingWrapPanel.GetOffset();
            }
        }

        public void RestoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty) || bookId.Equals(BeforeSearchPosition))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RestoreScrollOffsetInternal(bookId);
                });
            }
            else
            {
                RestoreScrollOffsetInternal(bookId);
            }

            _scrollOffset.Remove(bookId);
        }

        private void RestoreScrollOffsetInternal(Guid bookId)
        {
            var displayType = BookCabinet.DisplayType;
            if (displayType == DisplayType.SideBySide)
            {
                var virtualizingWrapPanel = Application.Current.MainWindow.FindChild<VirtualizingWrapPanel>("BookListViewVirtualinzingWrapPanel");
                if (_scrollOffset.ContainsKey(bookId))
                {
                    virtualizingWrapPanel.SetOffset(_scrollOffset[bookId]);
                }
                else
                {
                    virtualizingWrapPanel.ResetOffset();
                }
            }
            else if (displayType == DisplayType.Details)
            {
                var virtualizingStackPanel = Application.Current.MainWindow.FindChild<System.Windows.Controls.VirtualizingStackPanel>("BookListViewDetailsVirtualizingStackPanel");
                if (_scrollOffset.ContainsKey(bookId))
                {
                    virtualizingStackPanel.SetHorizontalOffset(_scrollOffset[bookId].X);
                    virtualizingStackPanel.SetVerticalOffset(_scrollOffset[bookId].Y);
                }
                else
                {
                    virtualizingStackPanel.SetHorizontalOffset(0);
                    virtualizingStackPanel.SetVerticalOffset(0);
                }
            }
        }

        public void ResetScrollOffset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var displayType = BookCabinet.DisplayType;
                if (displayType == DisplayType.SideBySide)
                {
                    var virtualizingWrapPanel = Application.Current.MainWindow.FindChild<VirtualizingWrapPanel>("BookListViewVirtualinzingWrapPanel");
                    virtualizingWrapPanel.ResetOffset();
                }
                else if (displayType == DisplayType.Details)
                {
                    var virtualizingStackPanel = Application.Current.MainWindow.FindChild<System.Windows.Controls.VirtualizingStackPanel>("BookListViewDetailsVirtualizingStackPanel");
                    virtualizingStackPanel.SetHorizontalOffset(0);
                    virtualizingStackPanel.SetVerticalOffset(0);
                }
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
            App.Current.Dispatcher.Invoke(() =>
            {
                BookListViewSelectedItems.Clear();
            });
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
            foreach (var item in MainWindowViewModel.Value.DockingDocumentViewModels.Where(t => !t.ContentId.Equals("home") && !t.ContentId.Equals(ContentId)))
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
                Command = ChangeStarCommand,
                CommandParameter = SelectedEntries.Where(x => x is BookViewModel).Cast<BookViewModel>().ToObservableCollection()
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
            menuitem.SetValue(RegionManager.RegionNameProperty, "ExtraBook");
            SelectManager.SelectedItems = SelectedEntries.Where(x => x is BookViewModel).Cast<object>().ToObservableCollection();
            SelectManager.ElementSelectedType = typeof(BookViewModel);
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
            menuitem.SetValue(RegionManager.RegionNameProperty, "ExtraPage");
            SelectManager.SelectedItems = SelectedEntries.Where(x => x is PageViewModel).Cast<object>().ToObservableCollection();
            SelectManager.ElementSelectedType = typeof(PageViewModel);
            menulist.Add(menuitem);

            ContentsContextMenuItems = menulist;
        }

        private void OpenBookPropertyDialog(BookViewModel book)
        {
            IDialogParameters parameter = new DialogParameters();
            BookFacade.FillContents(ref book);
            book.IsLoaded = true;
            book.ContentsRegistered = true;
            parameter.Add("Book", book);
            IDialogResult result = new DialogResult();
            dialogService.ShowDialog(nameof(BookProperty), parameter, ret => result = ret);
            if (result != null && result.Result == ButtonResult.OK)
            {

            }
        }

        private void OpenExportDialog(BookViewModel[] books)
        {
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("TargetBooks", books);
            IDialogResult result = new DialogResult();
            dialogService.ShowDialog(nameof(Export), parameters, ret => result = ret);
        }

        private static void OpenImageByDefaultProgram(IEnumerable<PageViewModel> images)
        {
            Process.Start(images.First().Image.AbsoluteMasterPath);
        }

        public async Task RemovePage(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.Value.RemovePages(pages.ToArray()).ConfigureAwait(false);
        }

        public async Task RemoveBook(BookViewModel[] books)
        {
            await LibraryManager.Value.RemoveBooks(books).ConfigureAwait(false);
        }

        private async Task ScrapPages(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.Value.ScrapPages(null, Specifications.SCRAPPED_NEW_BOOK_TITLE, pages.ToArray()).ConfigureAwait(false);
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
            await LibraryManager.Value.RemakeThumbnail(books).ConfigureAwait(false);
        }

        private async Task RemakeThumbnail(IEnumerable<PageViewModel> pages)
        {
            await LibraryManager.Value.RemakeThumbnail(pages).ConfigureAwait(false);
        }

        #endregion //サムネイル再作成

        #region 画面遷移

        public void OpenBook(BookViewModel book)
        {
            if (book == null) return;

            CloseSearchPane();
            StoreScrollOffset(Guid.Empty);
            BookFacade.FillContents(ref book);
            book.IsLoaded = true;
            book.ContentsRegistered = true;
            OpenedBook = book;
            RestoreScrollOffset(OpenedBook.ID);
            LibraryManager.Value.TagManager.ClearSelectedEntries();
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
                LibraryManager.Value.TagManager.ClearSelectedEntries();
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
            OpenedBook = LibraryManager.Value.OrderForward(page, OpenedBook);
        }

        public void MovePageBackward(PageViewModel page)
        {
            OpenedBook = LibraryManager.Value.OrderBackward(page, OpenedBook);
        }

        public async Task SaveOpenedBookContentsOrder()
        {
            Debug.Assert(OpenedBook is not null);
            await LibraryManager.Value.SaveBookContentsOrder(OpenedBook).ConfigureAwait(false);
        }

        #endregion //ページ単位ソート

        #region インポート

        public async Task ImportAsync(string[] objectPaths)
        {
            await LibraryManager.Value.ImportAsync(objectPaths).ConfigureAwait(false);
        }

        #endregion //インポート

        #region 問い合わせ

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(this.BookCabinet.Sorting, name);
        }

        public bool DisplayTypeSelected(string name)
        {
            return Querying.DisplayTypeSelected(this.BookCabinet.DisplayType, name);
        }

        #endregion //問い合わせ

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
