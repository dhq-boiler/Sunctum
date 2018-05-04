

using Ninject;
using NLog;
using Prism.Commands;
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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static Sunctum.UI.Core.Extensions;

namespace Sunctum.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private MainWindow _Parent;
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
        private ObservableCollection<System.Windows.Controls.Control> _TagContextMenuItems;
        private ObservableCollection<System.Windows.Controls.Control> _AuthorContextMenuItems;
        private IEnumerable<IPlugin> _Plugins;

        #region コマンド

        public ICommand AboutSunctumCommand { get; set; }

        public ICommand ClearSearchResultCommand { get; set; }

        public ICommand ClearResultSearchingByAuthorCommand { get; set; }

        public ICommand ClearResultSearchingByTagCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand ExportBooksCommand { get; set; }

        public ICommand GeneralCancelCommand { get; set; }

        public ICommand ImportFilesCommand { get; set; }

        public ICommand ImportFoldersCommand { get; set; }

        public ICommand ImportLibraryCommand { get; set; }

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

        public ICommand ScrapPagesCommand { get; set; }

        public ICommand SearchByAuthorCommand { get; set; }

        public ICommand SearchByTagCommand { get; set; }

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

        #endregion //コマンド

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

        public MainWindow ParentWindow
        {
            [DebuggerStepThrough]
            get
            { return _Parent; }
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

        public ObservableCollection<System.Windows.Controls.Control> TagContextMenuItems
        {
            get { return _TagContextMenuItems; }
            set { SetProperty(ref _TagContextMenuItems, value); }
        }

        public ObservableCollection<System.Windows.Controls.Control> AuthorContextMenuItems
        {
            get { return _AuthorContextMenuItems; }
            set { SetProperty(ref _AuthorContextMenuItems, value); }
        }

        [Inject]
        public IEnumerable<IPlugin> Plugins
        {
            get { return _Plugins; }
            set { SetProperty(ref _Plugins, value); }
        }

        [Inject]
        public IDataAccessManager DataAccessManager { get; set; }

        #endregion

        #region 一般

        public async Task Initialize(bool starting, bool shiftPressed = false)
        {
            SetMainWindow();

            if (starting)
            {
                BuildContextMenus_Books();
                BuildContextMenus_Contents();
                BuildContextMenus_Tags();
                BuildContextMenus_Authors();
                LoadPlugins();
            }

            if (shiftPressed || Configuration.ApplicationConfiguration.WorkingDirectory == null)
            {
                ChangeWorkingDirectory();
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

        private void BuildContextMenus_Authors()
        {
            AuthorContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "選択中の作者で検索",
                Command = SearchByAuthorCommand
            };
            AuthorContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "検索結果をクリア",
                Command = ClearResultSearchingByAuthorCommand
            };
            AuthorContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            AuthorContextMenuItems.Add(menuitem);
        }

        private void BuildContextMenus_Tags()
        {
            TagContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "選択中のタグで検索",
                Command = SearchByTagCommand
            };
            TagContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "検索結果をクリア",
                Command = ClearResultSearchingByTagCommand
            };
            TagContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            TagContextMenuItems.Add(menuitem);
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
                            TagContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                        case MenuType.MainWindow_Author_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Author_ContextMenu, () => LibraryVM.AuthorManager.SelectedItems.Where(e => e is AuthorViewModel).Cast<AuthorViewModel>()) as System.Windows.Controls.MenuItem;
                            AuthorContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
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
            ParentWindow.Dispatcher.Invoke(() =>
            {
                CloseSearchPane();
                CloseImage();
                CloseBook();
            });
            ResetScrollOffsetPool();
            ParentWindow.Dispatcher.Invoke(() => ResetScrollOffset());
            ParentWindow.Dispatcher.Invoke(() => SetProgress(1.0));
            ParentWindow.Dispatcher.Invoke(() => TooltipOnProgressBar = "Ready");

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

        private void SetMainWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _Parent = Application.Current.MainWindow as MainWindow;
            });
        }

        private void ProgressManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var progressManager = _LibraryVM.ProgressManager;
            var progress = progressManager.Progress;
            SetProgress(progress);
            TooltipOnProgressBar = progress == 1.0 ? "Ready" : $"{progress.ToString("0%")}{(progressManager.EstimateRemainTime.HasValue ? $" Estimate Remain Time {progressManager.EstimateRemainTime.Value.ToString(@"dd\.hh\:mm\:ss")}" : "")}";

            if (progressManager.IsAbort)
            {
                SetProgressAborted();
            }
        }

        private void LibraryVM_Searched(object sender, SearchedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PreviousSearchingText))
            {
                SaveScrollOffset(Guid.Empty);
            }

            ResetScrollOffset();
        }

        private void LibraryVM_SearchCleared(object sender, EventArgs e)
        {
            RestoreScrollOffset(Guid.Empty);
        }

        private bool ChangeWorkingDirectory()
        {
            var dialog = new FolderSelectDialog();
            dialog.Title = "ライブラリディレクトリの場所";

            return ParentWindow.Dispatcher.Invoke(() =>
            {
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
            });
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
                config.WindowRect = new Domain.Models.Rect(_Parent.Left, _Parent.Top, _Parent.Width, _Parent.Height);
            }
            else
            {
                config.WindowRect = null;
            }
            Configuration.Save(config);
        }

        public void Exit()
        {
            ParentWindow.Close();
        }

        #endregion //一般

        #region コマンド

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
            ClearResultSearchingByAuthorCommand = new DelegateCommand(() =>
            {
                ClearResultSearchingByAuthor();
            });
            ClearResultSearchingByTagCommand = new DelegateCommand(() =>
            {
                ClearResultSearchingByTag();
            });
            ExitApplicationCommand = new DelegateCommand(() =>
            {
                Exit();
            });
            ExportBooksCommand = new DelegateCommand(() =>
            {
                var books = _Parent.Book_ListView.SelectedItems.Cast<BookViewModel>();
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
            OpenAuthorManagementDialogCommand = new DelegateCommand(() =>
            {
                OpenAuthorManagementDialog();
            });
            OpenBookPropertyDialogCommand = new DelegateCommand(() =>
            {
                var books = _Parent.Book_ListView.SelectedItems.Cast<BookViewModel>();
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
                await OpenSwitchLibraryDialog();
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
                var books = _Parent.Book_ListView.SelectedItems.Cast<BookViewModel>();
                await RemakeThumbnail(books);
            });
            RemakeThumbnailOfPageCommand = new DelegateCommand(async () =>
            {
                var pages = _Parent.Contents_ListView.SelectedItems.Cast<PageViewModel>();
                await RemakeThumbnail(pages);
            });
            RemoveBookCommand = new DelegateCommand(async () =>
            {
                var books = _Parent.Book_ListView.SelectedItems.Cast<BookViewModel>();
                await RemoveBook(books.ToArray());
            });
            RemovePageCommand = new DelegateCommand<object>(async (p) =>
            {
                await RemovePage(p as IEnumerable<PageViewModel>);
            });
            ScrapPagesCommand = new DelegateCommand<object>(async (p) =>
            {
                await ScrapPages(p as IEnumerable<PageViewModel>);
            });
            SearchByAuthorCommand = new DelegateCommand(() =>
            {
                var selected = _Parent.Author_ListBox.SelectedItems;
                SearchByAuthor(selected.Cast<AuthorCountViewModel>());
            });
            SearchByTagCommand = new DelegateCommand(() =>
            {
                var selected = _Parent.Tag_ListBox.SelectedItems;
                SearchByTag(selected.Cast<TagCountViewModel>());
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
        }

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

        private void ClearResultSearchingByAuthor()
        {
            LibraryVM.ClearSearchResult();
        }

        private void SearchByAuthor(IEnumerable<AuthorCountViewModel> items)
        {
            LibraryVM.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            LibraryVM.AuthorManager.ShowBySelectedItems(LibraryVM, items.Select(ac => ac.Author));
            ResetScrollOffset();
        }

        private void ClearResultSearchingByTag()
        {
            LibraryVM.ClearSearchResult();
        }

        private void SearchByTag(IEnumerable<TagCountViewModel> items)
        {
            LibraryVM.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            LibraryVM.TagMng.ShowBySelectedItems(LibraryVM, items.Select(itc => itc.Tag));
            ResetScrollOffset();
        }

        private async Task OpenSwitchLibraryDialog()
        {
            bool changed = ChangeWorkingDirectory();
            if (changed)
            {
                await Initialize(false);
            }
        }

        public void ShowPreferenceDialog()
        {
            PreferencesDialog dialog = new PreferencesDialog();
            if (dialog.ShowDialog() == true)
            {
                bool willRestart = false;

                if (dialog.PreferencesVM.RestartRequired)
                {
                    willRestart = MessageBox.Show("変更を反映するには再起動が必要です.\n今すぐ再起動しますか？",
                        Process.GetCurrentProcess().MainWindowTitle, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK;
                }

                if (!dialog.PreferencesVM.RestartRequired || willRestart)
                {
                    Configuration.ApplicationConfiguration = dialog.PreferencesVM.Config;
                    Configuration.Save(Configuration.ApplicationConfiguration);
                }

                if (willRestart)
                {
                    Exit();
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                }
            }
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

        #endregion //コマンド

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

        public void FocusSearchBoxIf()
        {
            if (ParentWindow?.Search_TextBox == null) return;

            if (!ParentWindow.Search_TextBox.IsFocused)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(ParentWindow.Search_TextBox), ParentWindow.Search_TextBox);
                ParentWindow.Search_TextBox.SelectAll();
            }
        }

        private void OpenSearchPane()
        {
            ParentWindow.SearchPane_Border.Visibility = Visibility.Visible;
            FocusSearchBoxIf();
        }

        private void ClearSearchResult()
        {
            SearchText = "";
            LibraryVM.ClearSearchResult();
        }

        public void CloseSearchPane()
        {
            ParentWindow.SearchPane_Border.Visibility = Visibility.Collapsed;
        }

        #endregion //検索

        #region 画面遷移

        private VirtualizingWrapPanel VWP_Book
        {
            get
            {
                VirtualizingWrapPanel panel = null;
                ParentWindow.Dispatcher.Invoke(() =>
                {
                    panel = ParentWindow.Book_ListView.GetVisualChild<VirtualizingWrapPanel>();
                });
                return panel;
            }
        }

        private VirtualizingWrapPanel VWP_Contents
        {
            get
            {
                VirtualizingWrapPanel panel = null;
                ParentWindow.Dispatcher.Invoke(() =>
                {
                    panel = ParentWindow.Contents_ListView.GetVisualChild<VirtualizingWrapPanel>();
                });
                return panel;
            }
        }

        public void OpenBook(BookViewModel book)
        {
            if (book == null) return;

            CloseSearchPane();
            SaveScrollOffset(Guid.Empty);
            OpenedBook = book;
            RestoreScrollOffset(OpenedBook.ID);
            LibraryVM.TagMng.ClearSelectedEntries();
            Task.Factory.StartNew(() => LibraryVM.FireFillContents(book));
            ParentWindow.Book_ListView.Visibility = Visibility.Collapsed;
            ParentWindow.Contents_DockPanel.Visibility = Visibility.Visible;
        }

        public void CloseBook()
        {
            if (OpenedBook != null)
            {
                SaveScrollOffset(OpenedBook.ID);
                RestoreScrollOffset(Guid.Empty);
                LibraryVM.TagMng.ClearSelectedEntries();
            }
            OpenedBook = null;
            ParentWindow.Contents_DockPanel.Visibility = Visibility.Collapsed;
            ParentWindow.Book_ListView.Visibility = Visibility.Visible;
        }

        public void ResetScrollOffsetPool()
        {
            _scrollOffset = new Dictionary<Guid, Point>();
        }

        public void SaveScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;
            if (_scrollOffset.ContainsKey(bookId))
            {
                if (bookId.Equals(Guid.Empty))
                    _scrollOffset[bookId] = VWP_Book.GetOffset();
                else
                    _scrollOffset[bookId] = VWP_Contents.GetOffset();
            }
            else
            {
                if (bookId.Equals(Guid.Empty))
                    _scrollOffset.Add(bookId, VWP_Book.GetOffset());
                else
                    _scrollOffset.Add(bookId, VWP_Contents.GetOffset());
            }
        }

        public void RestoreScrollOffset(Guid bookId)
        {
            if (_scrollOffset == null) return;
            if (_scrollOffset.ContainsKey(bookId))
            {
                if (bookId.Equals(Guid.Empty))
                    VWP_Book.SetOffset(_scrollOffset[bookId]);
                else
                    VWP_Contents.SetOffset(_scrollOffset[bookId]);
            }
            else
            {
                if (bookId.Equals(Guid.Empty))
                    VWP_Book.ResetOffset();
                else
                    VWP_Contents.ResetOffset();
            }
        }

        public void ResetScrollOffset()
        {
            VWP_Book.ResetOffset();
            VWP_Contents.ResetOffset();
        }

        public void OpenImage(PageViewModel page)
        {
            OpenedPage = page;
            ParentWindow.Book_ListView.Visibility = Visibility.Collapsed;
            ParentWindow.Contents_DockPanel.Visibility = Visibility.Collapsed;
            ParentWindow.ImageViewer_DockPanel.Visibility = Visibility.Visible;
        }

        public void CloseImage()
        {
            _OpenedPage = null;
            ParentWindow.ImageViewer_DockPanel.Visibility = Visibility.Collapsed;
            ParentWindow.Book_ListView.Visibility = Visibility.Collapsed;
            ParentWindow.Contents_DockPanel.Visibility = Visibility.Visible;
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

        public void OperateByKeyboard(KeyEventArgs e)
        {
            if (OpenedBook == null)
            {
                BookView_OperateByKeyboard(e);
            }
            else if (OpenedPage == null)
            {
            }
            else
            {
                ImageView_OperateByKeyboard(e);
            }
        }

        private void BookView_OperateByKeyboard(KeyEventArgs e)
        {
            Debug.Assert(OpenedBook == null);
            Debug.Assert(OpenedPage == null);

            switch (e.Key)
            {
                case Key.Home:
                    if (LibraryVM.OnStage.Count() > 0)
                    {
                        var svAutomation = UIElementAutomationPeer.CreatePeerForElement(ParentWindow.Book_ListView);
                        var scrollInterface = svAutomation.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                        scrollInterface.SetScrollPercent(scrollInterface.HorizontalScrollPercent, 0.0);
                        Invoke(ParentWindow.Book_ListView, () => ParentWindow.Book_ListView.SelectedIndex = 0);
                        e.Handled = true;
                    }
                    break;
                case Key.End:
                    if (LibraryVM.OnStage.Count() > 0)
                    {
                        var svAutomation = UIElementAutomationPeer.CreatePeerForElement(ParentWindow.Book_ListView);
                        var scrollInterface = svAutomation.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                        scrollInterface.SetScrollPercent(scrollInterface.HorizontalScrollPercent, 100.0);
                        Invoke(ParentWindow.Book_ListView, () => ParentWindow.Book_ListView.SelectedIndex = LibraryVM.OnStage.Count() - 1);
                        e.Handled = true;
                    }
                    break;
                case Key.PageUp:
                    //VirtualizingWrapPanel.PageUp()
                    break;
                case Key.PageDown:
                    //VirtualizingWrapPanel.PageDown()
                    break;
            }
        }

        public void ImageView_OperateByKeyboard(KeyEventArgs e)
        {
            Debug.Assert(OpenedBook != null);
            Debug.Assert(OpenedPage != null);

            switch (e.Key)
            {
                case Key.Right:
                    GoNextImage();
                    BeginAnimation_Tick_NextImageButton();
                    e.Handled = true;
                    break;
                case Key.Left:
                    GoPreviousImage();
                    BeginAnimation_Tick_PreviousImageButton();
                    e.Handled = true;
                    break;
            }
        }

        private void BeginAnimation_Tick_PreviousImageButton()
        {
            var storyboard = (Storyboard)_Parent.FindResource("Storyboard_BlackWhite_Button_Color_Blink");
            storyboard.Begin(_Parent.GoBack_Button);
            var position = Mouse.GetPosition(_Parent.Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea);
            if (VisualTreeHelper.HitTest(_Parent.Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea, position) == null)
            {
                storyboard = (Storyboard)_Parent.FindResource("Storyboard_BlackWhite_Button_Opacity_Blink");
                storyboard.Begin(_Parent.GoBack_Button);
            }
        }

        private void BeginAnimation_Tick_NextImageButton()
        {
            var storyboard = (Storyboard)_Parent.FindResource("Storyboard_BlackWhite_Button_Color_Blink");
            storyboard.Begin(_Parent.GoNext_Button);
            var position = Mouse.GetPosition(_Parent.Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea);
            if (VisualTreeHelper.HitTest(_Parent.Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea, position) == null)
            {
                storyboard = (Storyboard)_Parent.FindResource("Storyboard_BlackWhite_Button_Opacity_Blink");
                storyboard.Begin(_Parent.GoNext_Button);
            }
        }

        #endregion //キーボード操作

        #region マウス操作

        public void OperateByMouseButton(MouseButtonEventArgs e)
        {
            if (OpenedBook == null)
            {
                BookView_OperateByMouseButton(e);
            }
            else if (OpenedPage == null)
            {
                ContentsView_OperateByMouseButton(e);
            }
            else
            {
                ImageView_OperateByMouseButton(e);
            }
        }

        private void BookView_OperateByMouseButton(MouseButtonEventArgs e)
        {
            Debug.Assert(OpenedBook == null);
            Debug.Assert(OpenedPage == null);

            switch (e.ChangedButton)
            {
                case MouseButton.XButton1: //マウス「戻る」ボタン
                    if (LibraryVM.IsSearching)
                    {
                        ClearSearchResult();
                        CloseSearchPane();
                    }
                    break;
                case MouseButton.XButton2: //マウス「進む」ボタン
                    break;
            }
        }

        private void ContentsView_OperateByMouseButton(MouseButtonEventArgs e)
        {
            Debug.Assert(OpenedBook != null);
            Debug.Assert(OpenedPage == null);

            switch (e.ChangedButton)
            {
                case MouseButton.XButton1: //マウス「戻る」ボタン
                    CloseBook();
                    e.Handled = true;
                    break;
                case MouseButton.XButton2: //マウス「進む」ボタン
                    break;
            }
        }

        private void ImageView_OperateByMouseButton(MouseButtonEventArgs e)
        {
            Debug.Assert(OpenedBook != null);
            Debug.Assert(OpenedPage != null);

            switch (e.ChangedButton)
            {
                case MouseButton.XButton1: //マウス「戻る」ボタン
                    CloseImage();
                    e.Handled = true;
                    break;
                case MouseButton.XButton2: //マウス「進む」ボタン
                    break;
            }
        }

        public void OperateByMouseWheel(MouseWheelEventArgs e)
        {
            if (OpenedBook == null)
            {
            }
            else if (OpenedPage == null)
            {
            }
            else
            {
                ImageView_OperateByMouseWheel(e);
            }
        }

        private void ImageView_OperateByMouseWheel(MouseWheelEventArgs e)
        {
            Debug.Assert(OpenedBook != null);
            Debug.Assert(OpenedPage != null);

            if (e.Delta > 0) //奥方向に回転
            {
                GoPreviousImage();
                e.Handled = true;
            }
            else if (e.Delta < 0) //手前方向に回転
            {
                GoNextImage();
                e.Handled = true;
            }
        }

        #endregion //マウス操作

        #region プログレスバー

        private double _memory_min0max1;

        private static readonly SolidColorBrush s_blue = new SolidColorBrush(Color.FromRgb(0x0, 0x7a, 0xcc));
        private static readonly SolidColorBrush s_orange = new SolidColorBrush(Color.FromRgb(0xff, 0x75, 0x0));
        private static readonly SolidColorBrush s_red = new SolidColorBrush(Colors.Red);
        private string _TooltipOnProgressBar;

        public void SetProgress(double min0max1)
        {
            Debug.Assert(min0max1 >= 0.0 && min0max1 <= 1.0);
            UpdateProgressBar(min0max1);
        }

        private void UpdateProgressBar(double min0max1)
        {
            _memory_min0max1 = min0max1;
            UpdateProgressBarLayout();
        }

        public void UpdateProgressBarLayout()
        {
            if (ParentWindow == null) return;

            ParentWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                double total = ParentWindow.Progress_DockPanel.ActualWidth;
                double left = total * _memory_min0max1;

                if (_memory_min0max1 < 1.0)
                    ParentWindow.Progress_Left_Grid.Background = s_orange;
                else
                    ParentWindow.Progress_Left_Grid.Background = s_blue;

                ParentWindow.Progress_Left_Grid.Width = left;
            }), DispatcherPriority.Render);
        }

        private void SetProgressAborted()
        {
            ParentWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                ParentWindow.Progress_Left_Grid.Background = s_red;
            }));
        }

        #endregion //プログレスバー
    }
}
