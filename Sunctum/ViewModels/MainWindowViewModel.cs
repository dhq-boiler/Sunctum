﻿

using Homura.Core;
using Homura.ORM;
using Homura.ORM.Setup;
using Ninject;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings.Extensions;
using Sunctum.Core.Notifications;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.AuthorSorting;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.ImageTagCountSorting;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
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
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Sunctum.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel, IDisposable, IObservable<ActiveTabChanged>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ILibrary _LibraryVM;
        private string _MainWindowTitle;
        private bool _DisplayTagPane;
        private bool _DisplayInformationPane;
        private bool _DisplayAuthorPane;
        private IEnumerable<IAddMenuPlugin> _Plugins;
        private string _TooltipOnProgressBar;
        private double _WindowLeft;
        private double _WindowTop;
        private double _WindowWidth;
        private double _WindowHeight;
        private CompositeDisposable _disposable = new CompositeDisposable();
        protected List<IObserver<ActiveTabChanged>> observerList = new List<IObserver<ActiveTabChanged>>();
        private ObservableCollection<IDocumentViewModelBase> _DockingDocumentViewModels;
        private ObservableCollection<PaneViewModelBase> _DockingPaneViewModels;
        private IDocumentViewModelBase _ActiveDocumentViewModel;

        #region コマンド

        public ICommand AboutSunctumCommand { get; set; }

        public ICommand ClearSearchResultCommand { get; set; }

        public ICommand EncryptionStartingCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand GeneralCancelCommand { get; set; }

        public ICommand ImportFilesCommand { get; set; }

        public ICommand ImportFoldersCommand { get; set; }

        public ICommand ImportLibraryCommand { get; set; }

        public ICommand OpenAuthorManagementDialogCommand { get; set; }

        public ICommand OpenMetadataImportSettingDialogCommand { get; set; }

        public ICommand OpenPowerSearchCommand { get; set; }

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

        public ICommand UnencryptionStartingCommand { get; set; }

        public ICommand UpdateBookByteSizeAllCommand { get; set; }

        public ICommand UpdateBookByteSizeStillNullCommand { get; set; }

        public ICommand UpdateBookTagCommand { get; set; }

        #endregion //コマンド

        public InteractionRequest<Notification> OpenPowerSearchRequest { get; } = new InteractionRequest<Notification>();

        #region コマンド登録

        private void RegisterCommands()
        {
            AboutSunctumCommand = new DelegateCommand(() =>
            {
                OpenAboutSunctumDialog();
            });
            ClearSearchResultCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.ClearSearchResult();
            });
            EncryptionStartingCommand = new DelegateCommand(async () =>
            {
                await OpenEncryptionStartingDialog();
            });
            ExitApplicationCommand = new DelegateCommand(() =>
            {
                Close();
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
            OpenPowerSearchCommand = new DelegateCommand(() =>
            {
                OpenPowerSearchRequest.Raise(new Notification() { Title = "Power search", Content = ActiveDocumentViewModel.BookCabinet });
            });
            OpenSwitchLibraryCommand = new DelegateCommand(async () =>
            {
                bool changed = OpenSwitchLibraryDialogAndChangeWorkingDirectory();
                if (changed)
                {
                    CloseAllTab();
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
                CloseAllTab();
                await LibraryVM.Reset();
                await Initialize(false);
            });
            ShowPreferenceDialogCommand = new DelegateCommand(() =>
            {
                ShowPreferenceDialog();
            });
            SortBookByAuthorAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByAuthorAsc;
            });
            SortBookByAuthorDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByAuthorDesc;
            });
            SortBookByCoverBlueAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverBlueAsc;
            });
            SortBookByCoverBlueDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverBlueDesc;
            });
            SortBookByCoverGreenAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverGreenAsc;
            });
            SortBookByCoverGreenDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverGreenDesc;
            });
            SortBookByCoverRedAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverRedAsc;
            });
            SortBookByCoverRedDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByCoverRedDesc;
            });
            SortBookByLoadedAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByLoadedAsc;
            });
            SortBookByLoadedDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByLoadedDesc;
            });
            SortBookByTitleAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByTitleAsc;
            });
            SortBookByTitleDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByTitleDesc;
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
                if (DisplayAuthorPane)
                {
                    if (!DockingPaneViewModels.Contains((PaneViewModelBase)AuthorPaneViewModel))
                    {
                        DockingPaneViewModels.Add((PaneViewModelBase)AuthorPaneViewModel);
                    }
                }
                else
                {
                    DockingPaneViewModels.Remove((PaneViewModelBase)AuthorPaneViewModel);
                }
            });
            ToggleDisplayInformationPaneCommand = new DelegateCommand(() =>
            {
                DisplayInformationPane = !DisplayInformationPane;
                if (DisplayInformationPane)
                {
                    if (!DockingPaneViewModels.Contains((PaneViewModelBase)InformationPaneViewModel))
                    {
                        DockingPaneViewModels.Add((PaneViewModelBase)InformationPaneViewModel);
                    }
                }
                else
                {
                    DockingPaneViewModels.Remove((PaneViewModelBase)InformationPaneViewModel);
                }
            });
            ToggleDisplayTagPaneCommand = new DelegateCommand(() =>
            {
                DisplayTagPane = !DisplayTagPane;
                if (DisplayTagPane)
                {
                    if (!DockingPaneViewModels.Contains((PaneViewModelBase)TagPaneViewModel))
                    {
                        DockingPaneViewModels.Add((PaneViewModelBase)TagPaneViewModel);
                    }
                }
                else
                {
                    DockingPaneViewModels.Remove((PaneViewModelBase)TagPaneViewModel);
                }
            });
            UnencryptionStartingCommand = new DelegateCommand(async () =>
            {
                await OpenUnencryptingDialog();
            });
            UpdateBookByteSizeAllCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeAll();
            });
            UpdateBookByteSizeStillNullCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeStillNull();
            });
            UpdateBookTagCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookTag();
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
        public ILibrary LibraryVM
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
        public IEnumerable<IAddMenuPlugin> Plugins
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

        [Inject]
        public ITagManager TagManager { get; set; }

        [Inject]
        public IAuthorManager AuthorManager { get; set; }

        public List<MenuItem> ExtraBookContextMenu { get; set; } = new List<MenuItem>();

        public List<MenuItem> ExtraPageContextMenu { get; set; } = new List<MenuItem>();

        public List<MenuItem> ExtraTagContextMenu { get; set; } = new List<MenuItem>();

        public List<MenuItem> ExtraAuthorContextMenu { get; set; } = new List<MenuItem>();

        public ObservableCollection<IDocumentViewModelBase> DockingDocumentViewModels
        {
            get { return _DockingDocumentViewModels; }
            set { SetProperty(ref _DockingDocumentViewModels, value); }
        }

        public ObservableCollection<PaneViewModelBase> DockingPaneViewModels
        {
            get { return _DockingPaneViewModels; }
            set { SetProperty(ref _DockingPaneViewModels, value); }
        }

        public IDocumentViewModelBase ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set
            {
                SetProperty(ref _ActiveDocumentViewModel, value);
                NotifyActiveTabChanged();
            }
        }

        public InteractionRequest<Notification> LoadLayoutRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> SaveLayoutRequest { get; } = new InteractionRequest<Notification>();

        public LayoutAnchorable AuthorPane { get; set; }

        public LayoutAnchorable TagPane { get; set; }

        public LayoutAnchorable InformationPane { get; set; }

        public Configuration Configuration { get { return Configuration.ApplicationConfiguration; } }

        #endregion

        #region 一般

        public async Task Initialize(bool starting, bool shiftPressed = false)
        {
            if (starting)
            {
                LibraryVM.ProgressManager.PropertyChanged += ProgressManager_PropertyChanged;
                TagPaneViewModel.BuildContextMenus_Tags();
                AuthorPaneViewModel.BuildContextMenus_Authors();
                LoadPlugins();
            }

            if (shiftPressed || Configuration.ApplicationConfiguration.WorkingDirectory == null)
            {
                OpenSwitchLibraryDialogAndChangeWorkingDirectory();
            }

            CloseAllTab();

            HomeDocumentViewModel.LibraryManager = LibraryVM;
            ((DocumentViewModelBase)HomeDocumentViewModel).MainWindowViewModel = this;

            var authorSorting = Configuration.ApplicationConfiguration.AuthorSorting;
            if (authorSorting != null)
            {
                AuthorManager.Sorting = AuthorSorting.GetReferenceByName(authorSorting);
            }

            var tagSorting = Configuration.ApplicationConfiguration.TagSorting;
            if (tagSorting != null)
            {
                TagManager.Sorting = ImageTagCountSorting.GetReferenceByName(tagSorting);
            }


            SetMainWindowTitle();
            HomeDocumentViewModel.ClearSearchResult();
            InitializeWindowComponent();
            ManageAppDB();

            Configuration.ApplicationConfiguration.ConnectionString = Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory);
            ConnectionManager.SetDefaultConnection(Configuration.ApplicationConfiguration.ConnectionString, typeof(SQLiteConnection));
            DataAccessManager.WorkingDao = new DaoBuilder(new Connection(Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory), typeof(SQLiteConnection)));

            await LibraryVM.Initialize();
            LibraryVM.UnlockIfLocked();
            await LibraryVM.Load()
                .ContinueWith(_ =>
                {
                    HomeDocumentViewModel.BookCabinet = LibraryVM.CreateBookStorage();

                    (LibraryVM as IObservable<BookCollectionChanged>)
                        .Subscribe(HomeDocumentViewModel.BookCabinet as IObserver<BookCollectionChanged>)
                        .AddTo(_disposable);
                    this.Subscribe((IObserver<ActiveTabChanged>)TagManager)
                        .AddTo(_disposable);
                    this.Subscribe((IObserver<ActiveTabChanged>)AuthorManager)
                        .AddTo(_disposable);

                    var sorting = Configuration.ApplicationConfiguration.BookSorting;
                    if (sorting != null)
                    {
                        HomeDocumentViewModel.BookCabinet.Sorting = BookSorting.GetReferenceByName(sorting);
                    }

                    ((DocumentViewModelBase)HomeDocumentViewModel).IsVisible = true;
                    ((DocumentViewModelBase)HomeDocumentViewModel).IsSelected = true;

                    SetEvent();

                    NotifyActiveTabChanged();
                });
        }

        public void NewSearchTab(ObservableCollection<BookViewModel> onStage)
        {
            var newTabViewModel = new SearchDocumentViewModel("Search results");
            newTabViewModel.LibraryManager = LibraryVM;
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource = new ObservableCollection<BookViewModel>(onStage);
            newTabViewModel.MainWindowViewModel = this;
            DockingDocumentViewModels.Add(newTabViewModel);

            (LibraryVM as IObservable<BookCollectionChanged>)
                .Subscribe(newTabViewModel.BookCabinet as IObserver<BookCollectionChanged>)
                .AddTo(_disposable);

            newTabViewModel.IsVisible = true;
            newTabViewModel.IsSelected = true;
            ActiveDocumentViewModel = newTabViewModel;
        }

        public void NewContentTab(BookViewModel bookViewModel)
        {
            var newTabViewModel = new ContentDocumentViewModel(bookViewModel.Title);
            newTabViewModel.LibraryManager = LibraryVM;
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource = new ObservableCollection<BookViewModel>();
            newTabViewModel.BookCabinet.BookSource.Add(bookViewModel);
            newTabViewModel.MainWindowViewModel = this;
            DockingDocumentViewModels.Add(newTabViewModel);

            (LibraryVM as IObservable<BookCollectionChanged>)
                .Subscribe(newTabViewModel.BookCabinet as IObserver<BookCollectionChanged>)
                .AddTo(_disposable);

            newTabViewModel.IsVisible = true;
            newTabViewModel.IsSelected = true;
            ActiveDocumentViewModel = newTabViewModel;
        }

        public void NewContentTab(IEnumerable<BookViewModel> list)
        {
            var newTabViewModel = new ContentDocumentViewModel("Filtered");
            newTabViewModel.LibraryManager = LibraryVM;
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource = new ObservableCollection<BookViewModel>();
            newTabViewModel.BookCabinet.BookSource.AddRange(list);
            newTabViewModel.MainWindowViewModel = this;
            DockingDocumentViewModels.Add(newTabViewModel);

            (LibraryVM as IObservable<BookCollectionChanged>)
                .Subscribe(newTabViewModel.BookCabinet as IObserver<BookCollectionChanged>)
                .AddTo(_disposable);

            newTabViewModel.IsVisible = true;
        }

        public void CloseTab(IDocumentViewModelBase documentViewModelBase)
        {
            DockingDocumentViewModels.Remove(documentViewModelBase);
        }

        public void CloseAllTab()
        {
            if (DockingDocumentViewModels == null) return;

            DockingDocumentViewModels.Clear();
        }

        private void NotifyActiveTabChanged()
        {
            if (observerList.Any())
            {
                foreach (var observer in observerList)
                {
                    if (ActiveDocumentViewModel != null)
                    {
                        observer.OnNext(new ActiveTabChanged(ActiveDocumentViewModel.BookCabinet));
                    }
                }
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
                            ExtraBookContextMenu.Add(menu);
                            break;
                        case MenuType.MainWindow_Page_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Page_ContextMenu, () => HomeDocumentViewModel.SelectedEntries.Where(e => e is PageViewModel).Cast<PageViewModel>()) as System.Windows.Controls.MenuItem;
                            ExtraPageContextMenu.Add(menu);
                            break;
                        case MenuType.MainWindow_Tag_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Tag_ContextMenu, () => TagManager.TagCount.Where(e => e is TagCountViewModel).Cast<TagCountViewModel>()) as System.Windows.Controls.MenuItem;
                            ExtraTagContextMenu.Add(menu);
                            break;
                        case MenuType.MainWindow_Author_ContextMenu:
                            menu = plugin.GetMenu(MenuType.MainWindow_Author_ContextMenu, () => AuthorManager.SelectedItems.Where(e => e is AuthorViewModel).Cast<AuthorViewModel>()) as System.Windows.Controls.MenuItem;
                            ExtraAuthorContextMenu.Add(menu);
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

        private void SetEvent()
        {
            HomeDocumentViewModel.BookCabinet.SearchCleared += LibraryVM_SearchCleared;
            HomeDocumentViewModel.BookCabinet.Searched += LibraryVM_Searched;
        }

        private void InitializeWindowComponent()
        {
            DisplayAuthorPane = Configuration.ApplicationConfiguration.DisplayAuthorPane;
            DisplayTagPane = Configuration.ApplicationConfiguration.DisplayTagPane;
            DisplayInformationPane = Configuration.ApplicationConfiguration.DisplayInformationPane;

            DockingDocumentViewModels = new ObservableCollection<IDocumentViewModelBase>();
            DockingDocumentViewModels.Add((HomeDocumentViewModel)HomeDocumentViewModel);

            DockingPaneViewModels = new ObservableCollection<PaneViewModelBase>();
            if (DisplayAuthorPane)
            {
                DockingPaneViewModels.Add((AuthorPaneViewModel)AuthorPaneViewModel);
            }
            if (DisplayTagPane)
            {
                DockingPaneViewModels.Add((TagPaneViewModel)TagPaneViewModel);
            }
            if (DisplayInformationPane)
            {
                DockingPaneViewModels.Add((InformationPaneViewModel)InformationPaneViewModel);
            }

            LoadLayoutRequest.Raise(new Notification() { Content = this });

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
            config.BookSorting = BookSorting.GetPropertyName(HomeDocumentViewModel.BookCabinet.Sorting);
            config.DisplayAuthorPane = DisplayAuthorPane;
            config.DisplayInformationPane = DisplayInformationPane;
            config.DisplayTagPane = DisplayTagPane;
            config.BookSorting = BookSorting.GetPropertyName(HomeDocumentViewModel.BookCabinet.Sorting);
            config.AuthorSorting = AuthorSorting.GetPropertyName(AuthorManager.Sorting);
            config.TagSorting = ImageTagCountSorting.GetPropertyName(TagManager.Sorting);

            if (config.StoreWindowPosition)
            {
                config.WindowRect = new Domain.Models.Rect(WindowLeft, WindowTop, WindowWidth, WindowHeight);
            }
            else
            {
                config.WindowRect = null;
            }
            Configuration.Save(config);
            SaveLayoutRequest.Raise(new Notification());

            Dispose();
        }

        public void Close()
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
                    var willDelete = TagManager.Chains.Where(t => t.TagID == id).ToList();
                    foreach (var del in willDelete)
                    {
                        TagManager.Chains.Remove(del);
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
                    var willUpdate = LibraryVM.BookSource.Where(b => b.AuthorID == target.ID);
                    foreach (var x in willUpdate)
                    {
                        x.Author = target.Clone() as AuthorViewModel;
                    }
                }),
                new Action<Guid>((id) =>
                {
                    AuthorFacade.Delete(id);
                    var willUpdate = LibraryVM.BookSource.Where(b => b.AuthorID == id);
                    foreach (var x in willUpdate)
                    {
                        x.Author = null;
                    }
                }),
                new Action<AuthorViewModel, AuthorViewModel>((willDiscard, into) =>
                {
                    AuthorFacade.Delete(willDiscard.ID);
                    var willUpdate = LibraryVM.BookSource.Where(b => b.AuthorID == willDiscard.ID);
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

        private async Task OpenEncryptionStartingDialog()
        {
            var dialog = new EncryptionStartingDialog();

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.StartEncryption(dialog.Password);
            }
        }

        private async Task OpenUnencryptingDialog()
        {
            var dialog = new InputPasswordDialog("暗号化を解除するにはパスワードを入力する必要があります。");

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.StartUnencryption(dialog.Password);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<ActiveTabChanged> observer)
        {
            observerList.Add(observer);
            return new ActiveTabChangedDisposable(this, observer);
        }

        private class ActiveTabChangedDisposable : IDisposable
        {
            private MainWindowViewModel _mainWindowViewModel;
            private readonly IObserver<ActiveTabChanged> _observer;

            public ActiveTabChangedDisposable(MainWindowViewModel mainWindowViewModel, IObserver<ActiveTabChanged> observer)
            {
                _mainWindowViewModel = mainWindowViewModel;
                _observer = observer;
            }

            public void Dispose()
            {
                _mainWindowViewModel.observerList.Remove(_observer);
            }
        }
    }
}
