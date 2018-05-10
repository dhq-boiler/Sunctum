

using Ninject;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.BookSorting;
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
using System.Windows;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ILibraryManager _LibraryVM;
        private string _MainWindowTitle;
        private bool _DisplayTagPane;
        private bool _DisplayInformationPane;
        private bool _DisplayAuthorPane;
        private IEnumerable<IPlugin> _Plugins;
        private string _TooltipOnProgressBar;
        private double _WindowLeft;
        private double _WindowTop;
        private double _WindowWidth;
        private double _WindowHeight;
        private ObservableCollection<DocumentViewModelBase> _TabItemViewModels;
        private int _SelectedTabIndex;

        #region コマンド

        public ICommand AboutSunctumCommand { get; set; }

        public ICommand ClearSearchResultCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand GeneralCancelCommand { get; set; }

        public ICommand ImportFilesCommand { get; set; }

        public ICommand ImportFoldersCommand { get; set; }

        public ICommand ImportLibraryCommand { get; set; }

        public ICommand OpenAuthorManagementDialogCommand { get; set; }

        public ICommand OpenMetadataImportSettingDialogCommand { get; set; }

        public ICommand OpenSwitchLibraryCommand { get; set; }

        public ICommand OpenTagManagementDialogCommand { get; set; }

        public ICommand ReloadLibraryCommand { get; set; }

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

        #region コマンド登録

        private void RegisterCommands()
        {
            AboutSunctumCommand = new DelegateCommand(() =>
            {
                OpenAboutSunctumDialog();
            });
            ClearSearchResultCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.ClearSearchResult();
            });
            ExitApplicationCommand = new DelegateCommand(() =>
            {
                Exit();
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
            OpenMetadataImportSettingDialogCommand = new DelegateCommand(() =>
            {
                OpenMetadataImportSettingDialog();
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
            ShowPreferenceDialogCommand = new DelegateCommand(() =>
            {
                ShowPreferenceDialog();
            });
            SortBookByAuthorAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByAuthorAsc;
            });
            SortBookByAuthorDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByAuthorDesc;
            });
            SortBookByCoverBlueAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverBlueAsc;
            });
            SortBookByCoverBlueDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverBlueDesc;
            });
            SortBookByCoverGreenAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverGreenAsc;
            });
            SortBookByCoverGreenDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverGreenDesc;
            });
            SortBookByCoverRedAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverRedAsc;
            });
            SortBookByCoverRedDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByCoverRedDesc;
            });
            SortBookByLoadedAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByLoadedAsc;
            });
            SortBookByLoadedDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByLoadedDesc;
            });
            SortBookByTitleAscCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByTitleAsc;
            });
            SortBookByTitleDescCommand = new DelegateCommand(() =>
            {
                HomeDocumentViewModel.Sorting = BookSorting.ByTitleDesc;
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

        #endregion //コマンド登録

        #region コンストラクタ

        public MainWindowViewModel()
        {
            RegisterCommands();
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

        public string MainWindowTitle
        {
            [DebuggerStepThrough]
            get
            { return _MainWindowTitle; }
            set { SetProperty(ref _MainWindowTitle, value); }
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

        public string TooltipOnProgressBar
        {
            get { return _TooltipOnProgressBar; }
            set { SetProperty(ref _TooltipOnProgressBar, value); }
        }

        [Inject]
        public IEnumerable<IPlugin> Plugins
        {
            get { return _Plugins; }
            set { SetProperty(ref _Plugins, value); }
        }

        [Inject]
        public IDataAccessManager DataAccessManager { get; set; }

        public InteractionRequest<Notification> CloseRequest { get; } = new InteractionRequest<Notification>();

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

        [Inject]
        public IAuthorPaneViewModel AuthorPaneViewModel { get; set; }

        [Inject]
        public ITagPaneViewModel TagPaneViewModel { get; set; }

        [Inject]
        public IInformationPaneViewModel InformationPaneViewModel { get; set; }

        [Inject]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        public ObservableCollection<DocumentViewModelBase> TabItemViewModels
        {
            get { return _TabItemViewModels; }
            set { SetProperty(ref _TabItemViewModels, value); }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set { SetProperty(ref _SelectedTabIndex, value); }
        }

        #endregion

        #region 一般

        public async Task Initialize(bool starting, bool shiftPressed = false)
        {
            if (starting)
            {
                HomeDocumentViewModel.BuildContextMenus_Books();
                HomeDocumentViewModel.BuildContextMenus_Contents();
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
                    HomeDocumentViewModel.Sorting = BookSorting.GetReferenceByName(sorting);
                }
            }

            TabItemViewModels = new ObservableCollection<DocumentViewModelBase>();
            TabItemViewModels.Add((DocumentViewModelBase)HomeDocumentViewModel);

            SelectedTabIndex = 0;
            ((DocumentViewModelBase)HomeDocumentViewModel).IsVisible = true;
            ((DocumentViewModelBase)HomeDocumentViewModel).IsSelected = true;

            SetMainWindowTitle();
            HomeDocumentViewModel.ClearSearchResult();
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
                            var menu = plugin.GetMenu(MenuType.MainWindow_Book_ContextMenu, () => HomeDocumentViewModel.SelectedEntries.Where(e => e is BookViewModel).Cast<BookViewModel>()) as System.Windows.Controls.MenuItem;
                            HomeDocumentViewModel.BooksContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
                                .Cast<System.Windows.Controls.MenuItem>()
                                .Single().Items.Add(menu);
                            break;
                        case MenuType.MainWindow_Page_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Page_ContextMenu, () => HomeDocumentViewModel.SelectedEntries.Where(e => e is PageViewModel).Cast<PageViewModel>()) as System.Windows.Controls.MenuItem;
                            HomeDocumentViewModel.ContentsContextMenuItems.Where(m => m is System.Windows.Controls.MenuItem && ((System.Windows.Controls.MenuItem)m).Header.Equals("Ex"))
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
            HomeDocumentViewModel.CloseSearchPane();
            HomeDocumentViewModel.CloseImage();
            HomeDocumentViewModel.CloseBook();
            HomeDocumentViewModel.ResetScrollOffsetPool();
            HomeDocumentViewModel.ResetScrollOffset();
            LibraryVM.ProgressManager.Complete();
            TooltipOnProgressBar = "Ready";
            HomeDocumentViewModel.ClearSelectedItems();
            AuthorPaneViewModel.ClearSelectedItems();
            TagPaneViewModel.ClearSelectedItems();

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
                HomeDocumentViewModel.StoreScrollOffset(Guid.Empty);
            }

            HomeDocumentViewModel.ResetScrollOffset();
        }

        private void LibraryVM_SearchCleared(object sender, EventArgs e)
        {
            HomeDocumentViewModel.RestoreScrollOffset(Guid.Empty);
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
            HomeDocumentViewModel.CloseSearchPane();
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
            config.BookSorting = BookSorting.GetPropertyName(HomeDocumentViewModel.Sorting);
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

        private async Task OpenImportFileDialogThenImport()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "ALL|*.*|JPEG file|*.jpg;*.jpeg|PNG file|*.png|GIF file|*.gif|BMP file|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await LibraryVM.ImportAsync(dialog.FileNames.ToArray());
            }
        }

        private async Task OpenImportFolderDialogThenImport()
        {
            var dialog = new FolderSelectDialog();

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.ImportAsync(new string[] { dialog.FileName });
            }
        }
    }
}
