

using Ninject;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Sunctum.Core.Extensions;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Setup;
using Sunctum.Plugin;
using Sunctum.UI.Controls;
using Sunctum.UI.Dialogs;
using Sunctum.UI.ViewModel;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ILibraryManager _LibraryVM;
        private BookViewModel _OpenedBook;
        private PageViewModel _OpenedPage;
        private string _MainWindowTitle;
        private string _SearchText;
        private Dictionary<Guid, Point> _scrollOffset;
        private bool _DisplayTagPane;
        private bool _DisplayInformationPane;
        private bool _DisplayAuthorPane;
        private List<EntryViewModel> _SelectedEntries;
        private ObservableCollection<System.Windows.Controls.Control> _ContentsContextMenuItems;
        private ObservableCollection<System.Windows.Controls.Control> _BooksContextMenuItems;
        private IEnumerable<IPlugin> _Plugins;
        private string _TooltipOnProgressBar;
        private bool _SearchPaneIsVisible;
        private string _ActiveContent;
        private List<BookViewModel> _BookListViewSelectedItems;
        private List<PageViewModel> _ContentsListViewSelectedItems;
        private double _WindowLeft;
        private double _WindowTop;
        private double _WindowWidth;
        private double _WindowHeight;

        #region コマンド

        public ICommand AboutSunctumCommand { get; set; }

        public ICommand ClearSearchResultCommand { get; set; }

        public ICommand CloseSearchPaneCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand ExportBooksCommand { get; set; }

        public ICommand GeneralCancelCommand { get; set; }

        public ICommand ImportFilesCommand { get; set; }

        public ICommand ImportFoldersCommand { get; set; }

        public ICommand ImportLibraryCommand { get; set; }

        public ICommand LeftKeyDownCommand { get; set; }

        public ICommand MouseWheelCommand { get; set; }

        public ICommand OpenAuthorManagementDialogCommand { get; set; }

        public ICommand OpenBookPropertyDialogCommand { get; set; }

        public ICommand OpenImageByDefaultProgramCommand { get; set; }

        public ICommand OpenMetadataImportSettingDialogCommand { get; set; }

        public ICommand OpenSearchPaneCommand { get; set; }

        public ICommand OpenSwitchLibraryCommand { get; set; }

        public ICommand OpenTagManagementDialogCommand { get; set; }

        public ICommand ReloadLibraryCommand { get; set; }

        public ICommand RemakeThumbnailOfBookCommand { get; set; }

        public ICommand RemakeThumbnailOfPageCommand { get; set; }

        public ICommand RemoveBookCommand { get; set; }

        public ICommand RemovePageCommand { get; set; }

        public ICommand RightKeyDownCommand { get; set; }

        public ICommand ScrapPagesCommand { get; set; }

        public ICommand ShowPreferenceDialogCommand { get; set; }

        public ICommand SortBookByAuthorAscCommand { get; set; }

        public ICommand SortBookByAuthorDescCommand { get; set; }

        public ICommand SortBookByCoverBlueAscCommand { get; set; }

        public ICommand SortBookByCoverBlueDescCommand { get; set; }

        public ICommand SortBookByCoverGreenAscCommand { get; set; }

        public ICommand SortBookByCoverGreenDescCommand { get; set; }

        public ICommand SortBookByCoverRedAscCommand { get; set; }

        public ICommand SortBookByCoverRedDescCommand { get; set; }

        public ICommand SortBookByLoadedAscCommand { get; set; }

        public ICommand SortBookByLoadedDescCommand { get; set; }

        public ICommand SortBookByTitleAscCommand { get; set; }

        public ICommand SortBookByTitleDescCommand { get; set; }

        public ICommand SwitchLibraryCommand { get; set; }

        public ICommand ToggleDisplayAuthorPaneCommand { get; set; }

        public ICommand ToggleDisplayInformationPaneCommand { get; set; }

        public ICommand ToggleDisplayTagPaneCommand { get; set; }

        public ICommand UpdateBookByteSizeAllCommand { get; set; }

        public ICommand UpdateBookByteSizeStillNullCommand { get; set; }

        public ICommand XButton1MouseButtonDownCommand { get; set; }

        public ICommand XButton2MouseButtonDownCommand { get; set; }

        #endregion //コマンド

        #region コマンド登録

        private void RegisterCommands()
        {
            AboutSunctumCommand = new DelegateCommand(() =>
            {
                OpenAboutSunctumDialog();
            });
            ClearSearchResultCommand = new DelegateCommand(() =>
            {
                ClearSearchResult();
            });
            CloseSearchPaneCommand = new DelegateCommand(() =>
            {
                CloseSearchPane();
            });
            ExitApplicationCommand = new DelegateCommand(() =>
            {
                Exit();
            });
            ExportBooksCommand = new DelegateCommand(() =>
            {
                var books = BookListViewSelectedItems;
                OpenExportDialog(books.ToArray());
            });
            GeneralCancelCommand = new DelegateCommand(() =>
            {
                GeneralCancel();
            });
            ImportFilesCommand = new DelegateCommand(async () =>
            {
                await OpenImportFileDialogThenImport();
            });
            ImportFoldersCommand = new DelegateCommand(async () =>
            {
                await OpenImportFolderDialogThenImport();
            });
            ImportLibraryCommand = new DelegateCommand(async () =>
            {
                await OpenImportLibraryDialog();
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
            OpenAuthorManagementDialogCommand = new DelegateCommand(() =>
            {
                OpenAuthorManagementDialog();
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
            OpenMetadataImportSettingDialogCommand = new DelegateCommand(() =>
            {
                OpenMetadataImportSettingDialog();
            });
            OpenSearchPaneCommand = new DelegateCommand(() =>
            {
                OpenSearchPane();
            });
            OpenSwitchLibraryCommand = new DelegateCommand(async () =>
            {
                bool changed = OpenSwitchLibraryDialogAndChangeWorkingDirectory();
                if (changed)
                {
                    await LibraryVM.Reset();
                    await Initialize(false);
                }
            });
            OpenTagManagementDialogCommand = new DelegateCommand(() =>
            {
                OpenTagManagementDialog();
            });
            ReloadLibraryCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.Reset();
                await Initialize(false);
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
            ScrapPagesCommand = new DelegateCommand<object>(async (p) =>
            {
                await ScrapPages(p as IEnumerable<PageViewModel>);
            });
            ShowPreferenceDialogCommand = new DelegateCommand(() =>
            {
                ShowPreferenceDialog();
            });
            SortBookByAuthorAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByAuthorAsc;
            });
            SortBookByAuthorDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByAuthorDesc;
            });
            SortBookByCoverBlueAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverBlueAsc;
            });
            SortBookByCoverBlueDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverBlueDesc;
            });
            SortBookByCoverGreenAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverGreenAsc;
            });
            SortBookByCoverGreenDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverGreenDesc;
            });
            SortBookByCoverRedAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverRedAsc;
            });
            SortBookByCoverRedDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByCoverRedDesc;
            });
            SortBookByLoadedAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByLoadedAsc;
            });
            SortBookByLoadedDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByLoadedDesc;
            });
            SortBookByTitleAscCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByTitleAsc;
            });
            SortBookByTitleDescCommand = new DelegateCommand(() =>
            {
                LibraryVM.Sorting = BookSorting.ByTitleDesc;
            });
            SwitchLibraryCommand = new DelegateCommand<RecentOpenedLibrary>(async (p) =>
            {
                await LibraryVM.Reset();
                Configuration.ApplicationConfiguration.WorkingDirectory = p.Path;
                Configuration.Save(Configuration.ApplicationConfiguration);
                await Initialize(false);
            });
            ToggleDisplayAuthorPaneCommand = new DelegateCommand(() =>
            {
                DisplayAuthorPane = !DisplayAuthorPane;
            });
            ToggleDisplayInformationPaneCommand = new DelegateCommand(() =>
            {
                DisplayInformationPane = !DisplayInformationPane;
            });
            ToggleDisplayTagPaneCommand = new DelegateCommand(() =>
            {
                DisplayTagPane = !DisplayTagPane;
            });
            UpdateBookByteSizeAllCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeAll();
            });
            UpdateBookByteSizeStillNullCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeStillNull();
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
                    if (LibraryVM.IsSearching)
                    {
                        ClearSearchResult();
                        CloseSearchPane();
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

        #endregion //コマンド登録

        #region コンストラクタ

        public MainWindowViewModel()
        {
            RegisterCommands();
            SelectedEntries = new List<EntryViewModel>();
        }

        #endregion

        #region プロパティ

        public System.Version SunctumVersion
        {
            [DebuggerStepThrough]
            get
            { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        [Inject]
        public ILibraryManager LibraryVM
        {
            [DebuggerStepThrough]
            get
            { return _LibraryVM; }
            set { SetProperty(ref _LibraryVM, value); }
        }

        public List<EntryViewModel> SelectedEntries
        {
            [DebuggerStepThrough]
            get
            { return _SelectedEntries; }
            protected set { SetProperty(ref _SelectedEntries, value); }
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

        public string MainWindowTitle
        {
            [DebuggerStepThrough]
            get
            { return _MainWindowTitle; }
            set { SetProperty(ref _MainWindowTitle, value); }
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

        public bool DisplayTagPane
        {
            [DebuggerStepThrough]
            get
            { return _DisplayTagPane; }
            set { SetProperty(ref _DisplayTagPane, value); }
        }

        public bool DisplayInformationPane
        {
            [DebuggerStepThrough]
            get
            { return _DisplayInformationPane; }
            set { SetProperty(ref _DisplayInformationPane, value); }
        }

        public bool DisplayAuthorPane
        {
            [DebuggerStepThrough]
            get
            { return _DisplayAuthorPane; }
            set { SetProperty(ref _DisplayAuthorPane, value); }
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

        public string TooltipOnProgressBar
        {
            get { return _TooltipOnProgressBar; }
            set { SetProperty(ref _TooltipOnProgressBar, value); }
        }

        public ObservableCollection<System.Windows.Controls.Control> BooksContextMenuItems
        {
            get { return _BooksContextMenuItems; }
            set { SetProperty(ref _BooksContextMenuItems, value); }
        }

        public ObservableCollection<System.Windows.Controls.Control> ContentsContextMenuItems
        {
            get { return _ContentsContextMenuItems; }
            set { SetProperty(ref _ContentsContextMenuItems, value); }
        }

        [Inject]
        public IEnumerable<IPlugin> Plugins
        {
            get { return _Plugins; }
            set { SetProperty(ref _Plugins, value); }
        }

        [Inject]
        public IDataAccessManager DataAccessManager { get; set; }

        public bool SearchPaneIsVisible
        {
            get { return _SearchPaneIsVisible; }
            set { SetProperty(ref _SearchPaneIsVisible, value); }
        }

        public string ActiveContent
        {
            get { return _ActiveContent; }
            set { SetProperty(ref _ActiveContent, value); }
        }

        public InteractionRequest<Notification> CloseRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> ResetScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> StoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreBookScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> RestoreContentScrollOffsetRequest { get; } = new InteractionRequest<Notification>();

        public List<BookViewModel> BookListViewSelectedItems
        {
            get { return _BookListViewSelectedItems; }
            set { SetProperty(ref _BookListViewSelectedItems, value); }
        }

        public List<PageViewModel> ContentsListViewSelectedItems
        {
            get { return _ContentsListViewSelectedItems; }
            set { SetProperty(ref _ContentsListViewSelectedItems, value); }
        }

        public double WindowLeft
        {
            get { return _WindowLeft; }
            set { SetProperty(ref _WindowLeft, value); }
        }

        public double WindowTop
        {
            get { return _WindowTop; }
            set { SetProperty(ref _WindowTop, value); }
        }

        public double WindowWidth
        {
            get { return _WindowWidth; }
            set { SetProperty(ref _WindowWidth, value); }
        }

        public double WindowHeight
        {
            get { return _WindowHeight; }
            set { SetProperty(ref _WindowHeight, value); }
        }

        public InteractionRequest<Notification> BlinkGoNextButtonRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> BlinkGoBackButtonRequest { get; } = new InteractionRequest<Notification>();

        [Inject]
        public IAuthorPaneViewModel AuthorPaneViewModel { get; set; }

        [Inject]
        public ITagPaneViewModel TagPaneViewModel { get; set; }

        #endregion

        #region 一般

        public async Task Initialize(bool starting, bool shiftPressed = false)
        {
            if (starting)
            {
                BuildContextMenus_Books();
                BuildContextMenus_Contents();
                TagPaneViewModel.BuildContextMenus_Tags();
                AuthorPaneViewModel.BuildContextMenus_Authors();
                LoadPlugins();
            }

            if (shiftPressed || Configuration.ApplicationConfiguration.WorkingDirectory == null)
            {
                OpenSwitchLibraryDialogAndChangeWorkingDirectory();
            }

            if (starting)
            {
                var sorting = Configuration.ApplicationConfiguration.BookSorting;
                if (sorting != null)
                {
                    LibraryVM.Sorting = BookSorting.GetReferenceByName(sorting);
                }
            }

            SetMainWindowTitle();
            ClearSearchResult();
            InitializeWindowComponent();
            SetEventToLibraryManager();
            ManageAppDB();

            Configuration.ApplicationConfiguration.ConnectionString = Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory);
            ConnectionManager.SetDefaultConnection(Configuration.ApplicationConfiguration.ConnectionString, typeof(SQLiteConnection));
            DataAccessManager.WorkingDao = new DaoBuilder(new Connection(Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory), typeof(SQLiteConnection)));

            try
            {
                await LibraryVM.Initialize();
                await LibraryVM.Load();
            }
            catch (FailedOpeningDatabaseException)
            {
                Exit();
            }
        }

        private void ManageAppDB()
        {
            DataVersionManager dvManager = new DataVersionManager();
            dvManager.CurrentConnection = DataAccessManager.AppDao.CurrentConnection;
            dvManager.Mode = VersioningStrategy.ByTick;
            dvManager.RegisterChangePlan(new ChangePlan_AppDb_VersionOrigin());
            dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo;

            dvManager.UpgradeToTargetVersion();
        }

        private void DvManager_FinishedToUpgradeTo(object sender, ModifiedEventArgs e)
        {
            s_logger.Info($"Heavy Modifying AppDB Count : {e.ModifiedCount}");

            if (e.ModifiedCount > 0)
            {
                SQLiteBaseDao<Dummy>.Vacuum(DataAccessManager.AppDao.CurrentConnection);
            }
        }

        private void BuildContextMenus_Books()
        {
            BooksContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "書き出し",
                Command = ExportBooksCommand
            };
            BooksContextMenuItems.Add(menuitem);

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
            BooksContextMenuItems.Add(manageMenu);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "プロパティ",
                Command = OpenBookPropertyDialogCommand,
            };
            BooksContextMenuItems.Add(menuitem);

            BooksContextMenuItems.Add(new System.Windows.Controls.Separator());

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            BooksContextMenuItems.Add(menuitem);
        }

        private void BuildContextMenus_Contents()
        {
            ContentsContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "開く",
                Command = OpenImageByDefaultProgramCommand,
                CommandParameter = SelectedEntries.Where(x => x is PageViewModel).Cast<PageViewModel>()
            };
            ContentsContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "スクラップ",
                Command = ScrapPagesCommand,
                CommandParameter = SelectedEntries.Where(x => x is PageViewModel).Cast<PageViewModel>()
            };
            ContentsContextMenuItems.Add(menuitem);

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
            ContentsContextMenuItems.Add(manageMenu);

            ContentsContextMenuItems.Add(new System.Windows.Controls.Separator());

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            ContentsContextMenuItems.Add(menuitem);
        }

        private void LoadPlugins()
        {
            foreach (var plugin in Plugins)
            {
                var flags = plugin.Where().GetFlags();
                foreach (var flag in flags)
                {
                    switch (flag)
                    {
                        case MenuType.MainWindow_Book_ContextMenu:
                            var menu = plugin.GetMenu(MenuType.MainWindow_Book_ContextMenu, () => SelectedEntries.Where(e => e is BookViewModel).Cast<BookViewModel>()) as System.Windows.Controls.MenuItem;
                            BooksContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                        case MenuType.MainWindow_Page_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Page_ContextMenu, () => SelectedEntries.Where(e => e is PageViewModel).Cast<PageViewModel>()) as System.Windows.Controls.MenuItem;
                            ContentsContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                        case MenuType.MainWindow_Tag_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Tag_ContextMenu, () => LibraryVM.TagMng.TagCount.Where(e => e is TagCountViewModel).Cast<TagCountViewModel>()) as System.Windows.Controls.MenuItem;
                            TagPaneViewModel.TagContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                        case MenuType.MainWindow_Author_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Author_ContextMenu, () => LibraryVM.AuthorManager.SelectedItems.Where(e => e is AuthorViewModel).Cast<AuthorViewModel>()) as System.Windows.Controls.MenuItem;
                            AuthorPaneViewModel.AuthorContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                    }
                }

                var assembly = Assembly.GetAssembly(plugin.GetType());
                var codebase = assembly.GetName().CodeBase.ToString();
                var uri = new UriBuilder(codebase);
                var path = Uri.UnescapeDataString(uri.Path);
                s_logger.Info($"Plugin Loaded '{plugin.GetType().Name}' from '{path}' {assembly.GetName().Version.ToString()}");
            }
        }

        private void SetEventToLibraryManager()
        {
            LibraryVM.ProgressManager.PropertyChanged += ProgressManager_PropertyChanged;
            LibraryVM.SearchCleared += LibraryVM_SearchCleared;
            LibraryVM.Searched += LibraryVM_Searched;
        }

        private void InitializeWindowComponent()
        {
            CloseSearchPane();
            CloseImage();
            CloseBook();
            ResetScrollOffsetPool();
            ResetScrollOffset();
            LibraryVM.ProgressManager.Complete();
            TooltipOnProgressBar = "Ready";
            BookListViewSelectedItems = new List<BookViewModel>();
            AuthorPaneViewModel.ClearSelectedItems();
            TagPaneViewModel.TagListBoxSelectedItems = new List<TagCountViewModel>();

            DisplayAuthorPane = Configuration.ApplicationConfiguration.DisplayAuthorPane;
            DisplayTagPane = Configuration.ApplicationConfiguration.DisplayTagPane;
            DisplayInformationPane = Configuration.ApplicationConfiguration.DisplayInformationPane;
        }

        private void SetMainWindowTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var majorMinorBuild = $"{version.Major}.{version.Minor}.{version.Build}";
            var appnameAndVersion = $"Sunctum {majorMinorBuild}";

            if (Configuration.ApplicationConfiguration != null && Configuration.ApplicationConfiguration.WorkingDirectory != null)
            {
                MainWindowTitle = $"{Configuration.ApplicationConfiguration.WorkingDirectory} - {appnameAndVersion}";
            }
            else
            {
                MainWindowTitle = appnameAndVersion;
            }
        }

        private void ProgressManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var progressManager = _LibraryVM.ProgressManager;
            var progress = progressManager.Progress;
            TooltipOnProgressBar = progress == 1.0 ? "Ready" : $"{progress.ToString("0%")}{(progressManager.EstimateRemainTime.HasValue ? $" Estimate Remain Time {progressManager.EstimateRemainTime.Value.ToString(@"dd\.hh\:mm\:ss")}" : "")}";
        }

        private void LibraryVM_Searched(object sender, SearchedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PreviousSearchingText))
            {
                StoreScrollOffset(Guid.Empty);
            }

            ResetScrollOffset();
        }

        private void LibraryVM_SearchCleared(object sender, EventArgs e)
        {
            RestoreScrollOffset(Guid.Empty);
        }

        private bool OpenSwitchLibraryDialogAndChangeWorkingDirectory()
        {
            var dialog = new FolderSelectDialog();
            dialog.Title = "ライブラリディレクトリの場所";
            if (dialog.ShowDialog() == true)
            {
                Configuration.ApplicationConfiguration.WorkingDirectory = dialog.FileName;
                Configuration.Save(Configuration.ApplicationConfiguration);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GeneralCancel()
        {
            CloseSearchPane();
        }

        private static void Invoke(UIElement elm, Action act)
        {
            if (elm.CheckAccess())
            {
                act.Invoke();
            }
            else
            {
                elm.Dispatcher.Invoke(act);
            }
        }

        public void Terminate()
        {
            var config = Configuration.ApplicationConfiguration;
            config.BookSorting = BookSorting.GetPropertyName(LibraryVM.Sorting);
            config.DisplayAuthorPane = DisplayAuthorPane;
            config.DisplayInformationPane = DisplayInformationPane;
            config.DisplayTagPane = DisplayTagPane;
            if (config.StoreWindowPosition)
            {
                config.WindowRect = new Domain.Models.Rect(WindowLeft, WindowTop, WindowWidth, WindowHeight);
            }
            else
            {
                config.WindowRect = null;
            }
            Configuration.Save(config);
        }

        public void Exit()
        {
            CloseRequest.Raise(new Notification());
        }

        #endregion //一般

        private void OpenAboutSunctumDialog()
        {
            var dialog = new AboutSunctumDialog();
            dialog.ShowDialog();
        }

        private void OpenMetadataImportSettingDialog()
        {
            MetadataImportSettingDialog dialog = new MetadataImportSettingDialog();
            dialog.ShowDialog();
        }

        private void OpenTagManagementDialog()
        {
            EntityManagementDialog<TagViewModel> dialog = new EntityManagementDialog<TagViewModel>();
            EntityManagementDialogViewModel<TagViewModel> dialogViewModel = new EntityManagementDialogViewModel<TagViewModel>(dialog, LibraryVM, "タグの管理",
                new Func<string, TagViewModel>((name) =>
                {
                    var tag = new TagViewModel();
                    tag.ID = Guid.NewGuid();
                    tag.UnescapedName = name;
                    TagFacade.Insert(tag);
                    return tag;
                }),
                new Func<IEnumerable<TagViewModel>>(() =>
                {
                    return TagFacade.OrderByNaturalString();
                }),
                new Func<Guid, TagViewModel>((id) =>
                {
                    return TagFacade.FindBy(id);
                }),
                null,
                new Action<Guid>((id) =>
                {
                    TagFacade.Delete(id);
                    var willDelete = LibraryVM.TagMng.Chains.Where(t => t.TagID == id).ToList();
                    foreach (var del in willDelete)
                    {
                        LibraryVM.TagMng.Chains.Remove(del);
                    }
                }),
                null);
            dialog.EntityMngVM = dialogViewModel;
            dialogViewModel.Initialize();
            dialog.Show();
        }

        public void ShowPreferenceDialog()
        {
            PreferencesDialog dialog = new PreferencesDialog();
            dialog.ShowDialog();
        }

        private async Task OpenImportLibraryDialog()
        {
            var dialog = new FolderSelectDialog
            {
                Title = "ライブラリディレクトリの場所"
            };

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.ImportLibrary(dialog.FileName);
            }
        }

        public async Task RemovePage(IEnumerable<PageViewModel> pages)
        {
            await LibraryVM.RemovePages(pages.ToArray());
        }

        public async Task RemoveBook(BookViewModel[] books)
        {
            await LibraryVM.RemoveBooks(books);
        }

        private void OpenExportDialog(BookViewModel[] books)
        {
            ExportDialog dialog = new ExportDialog(LibraryVM, books);
            dialog.ShowDialog();
        }

        private void OpenAuthorManagementDialog()
        {
            EntityManagementDialog<AuthorViewModel> dialog = new EntityManagementDialog<AuthorViewModel>();
            EntityManagementDialogViewModel<AuthorViewModel> dialogViewModel = new EntityManagementDialogViewModel<AuthorViewModel>(dialog, LibraryVM, "Authorの管理",
                new Func<string, AuthorViewModel>((name) =>
                {
                    var author = new AuthorViewModel();
                    author.ID = Guid.NewGuid();
                    author.UnescapedName = name;
                    AuthorFacade.Create(author);
                    return author;
                }),
                new Func<IEnumerable<AuthorViewModel>>(() =>
                {
                    return AuthorFacade.OrderByNaturalString();
                }),
                new Func<Guid, AuthorViewModel>((id) =>
                {
                    return AuthorFacade.FindBy(id);
                }),
                new Action<AuthorViewModel>((target) =>
                {
                    AuthorFacade.Update(target);
                    var willUpdate = LibraryVM.LoadedBooks.Where(b => b.AuthorID == target.ID);
                    foreach (var x in willUpdate)
                    {
                        x.Author = target.Clone() as AuthorViewModel;
                    }
                }),
                new Action<Guid>((id) =>
                {
                    AuthorFacade.Delete(id);
                    var willUpdate = LibraryVM.LoadedBooks.Where(b => b.AuthorID == id);
                    foreach (var x in willUpdate)
                    {
                        x.Author = null;
                    }
                }),
                new Action<AuthorViewModel, AuthorViewModel>((willDiscard, into) =>
                {
                    AuthorFacade.Delete(willDiscard.ID);
                    var willUpdate = LibraryVM.LoadedBooks.Where(b => b.AuthorID == willDiscard.ID);
                    foreach (var x in willUpdate)
                    {
                        x.Author = into.Clone() as AuthorViewModel;
                        BookFacade.Update(x);
                    }
                }));
            dialog.EntityMngVM = dialogViewModel;
            dialogViewModel.Initialize();
            dialog.Show();
        }

        private void OpenBookPropertyDialog(BookViewModel book)
        {
            BookPropertyDialog dialog = new BookPropertyDialog(book);
            dialog.ShowDialog();
        }

        private async Task ScrapPages(IEnumerable<PageViewModel> pages)
        {
            await LibraryVM.ScrapPages(null, Specifications.SCRAPPED_NEW_BOOK_TITLE, pages.ToArray());
        }

        private static void OpenImageByDefaultProgram(IEnumerable<PageViewModel> images)
        {
            Process.Start(images.First().Image.AbsoluteMasterPath);
        }

        private async Task OpenImportFileDialogThenImport()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "ALL|*.*|JPEG file|*.jpg;*.jpeg|PNG file|*.png|GIF file|*.gif|BMP file|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await ImportAsync(dialog.FileNames.ToArray());
            }
        }

        private async Task OpenImportFolderDialogThenImport()
        {
            var dialog = new FolderSelectDialog();

            if (dialog.ShowDialog() == true)
            {
                await ImportAsync(new string[] { dialog.FileName });
            }
        }

        private async Task RemakeThumbnails(IEnumerable<PageViewModel> obj)
        {
            if (obj.Count() > 0)
            {
                var first = obj.ElementAt(0);
                string typeName = first.GetType().Name;
                switch (typeName)
                {
                    case nameof(BookViewModel):
                        await RemakeThumbnail(obj.Cast<BookViewModel>());
                        break;
                    case nameof(PageViewModel):
                        await RemakeThumbnail(obj.Cast<PageViewModel>());
                        break;
                }
            }
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

        #region サムネイル再作成

        private async Task RemakeThumbnail(IEnumerable<BookViewModel> books)
        {
            await LibraryVM.RemakeThumbnail(books);
        }

        private async Task RemakeThumbnail(IEnumerable<PageViewModel> pages)
        {
            await LibraryVM.RemakeThumbnail(pages);
        }

        #endregion //サムネイル再作成

        #region 検索

        public void Search()
        {
            LibraryVM.Search(SearchText);
        }

        public void Search(string searchText)
        {
            SearchText = searchText;
        }

        private void OpenSearchPane()
        {
            SearchPaneIsVisible = true;
        }

        private void ClearSearchResult()
        {
            SearchText = "";
            LibraryVM.ClearSearchResult();
        }

        public void CloseSearchPane()
        {
            SearchPaneIsVisible = false;
        }

        #endregion //検索

        #region 画面遷移

        public void OpenBook(BookViewModel book)
        {
            if (book == null) return;

            CloseSearchPane();
            StoreScrollOffset(Guid.Empty);
            OpenedBook = book;
            RestoreScrollOffset(OpenedBook.ID);
            LibraryVM.TagMng.ClearSelectedEntries();
            Task.Factory.StartNew(() => LibraryVM.FireFillContents(book));
            ActiveContent = "PageView";
        }

        public void CloseBook()
        {
            ActiveContent = "BookView";
            if (OpenedBook != null)
            {
                StoreScrollOffset(OpenedBook.ID);
                RestoreScrollOffset(Guid.Empty);
                LibraryVM.TagMng.ClearSelectedEntries();
            }
            OpenedBook = null;
        }

        public void ResetScrollOffsetPool()
        {
            _scrollOffset = new Dictionary<Guid, Point>();
        }

        public void StoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;

            if (bookId.Equals(Guid.Empty))
            {
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

            if (bookId.Equals(Guid.Empty))
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
        }

        public void ResetScrollOffset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ResetScrollOffsetRequest.Raise(new Notification());
            });
        }

        public void OpenImage(PageViewModel page)
        {
            OpenedPage = page;
            ActiveContent = "ImageView";
        }

        public void CloseImage()
        {
            OpenedPage = null;
            ActiveContent = "PageView";
        }

        #endregion

        #region インポート

        public async Task ImportAsync(string[] objectPaths)
        {
            await LibraryVM.ImportAsync(objectPaths);
        }

        #endregion //インポート

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
            OpenedBook = LibraryVM.OrderForward(page, OpenedBook);
        }

        public void MovePageBackward(PageViewModel page)
        {
            OpenedBook = LibraryVM.OrderBackward(page, OpenedBook);
        }

        public async Task SaveOpenedBookContentsOrder()
        {
            await LibraryVM.SaveBookContentsOrder(OpenedBook);
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
    }
}
