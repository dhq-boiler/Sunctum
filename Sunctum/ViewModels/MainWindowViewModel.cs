﻿

using ChinhDo.Transactions;
using Homura.Core;
using Homura.ORM;
using Homura.ORM.Setup;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Core.Extensions;
using Sunctum.Core.Notifications;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.AuthorSorting;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.DisplayType;
using Sunctum.Domain.Logic.Encrypt;
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
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;
using Unity;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

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
        private IDocumentViewModelBase _OldActiveDocumentViewModel;

        #region コンストラクタ

        public MainWindowViewModel(IDialogService DialogService)
        {
            RegisterCommands();
            this.DialogService = DialogService;
        }

        #endregion

        #region コマンド

        public ICommand AboutSunctumCommand { get; set; }

        public ICommand ClearSearchResultCommand { get; set; }

        public ICommand DisplaySideBySideCommand { get; set; }

        public ICommand RemoveHistoryCommand { get; set; }

        public ICommand DetailsCommand { get; set; }

        public ICommand EncryptionStartingCommand { get; set; }

        public ICommand EncryptionContinuingCommand { get; set; }

        public ICommand ExitApplicationCommand { get; set; }

        public ICommand GeneralCancelCommand { get; set; }

        public ICommand ImportFilesCommand { get; set; }

        public ICommand ImportFoldersCommand { get; set; }

        public ICommand ImportLibraryCommand { get; set; }

        public ICommand LoadedCommand { get; set; }

        public ICommand OpenAuthorManagementDialogCommand { get; set; }

        public ICommand OpenMetadataImportSettingDialogCommand { get; set; }

        public ICommand OpenPowerSearchCommand { get; set; }

        public ICommand OpenStatisticsDialogCommand { get; set; }

        public ICommand OpenSwitchLibraryCommand { get; set; }

        public ICommand OpenSearchPaneCommand { get; set; }

        public ICommand OpenTagManagementDialogCommand { get; set; }

        public ICommand ReloadLibraryCommand { get; set; }

        public ICommand ReplaceContentCommand { get; set; }

        public ICommand ShowPreferenceDialogCommand { get; set; }

        public ICommand ShowDuplicateBooksCommand { get; set; }

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

        public ICommand SortBookByFingerPrintAscCommand { get; set; }

        public ICommand SortBookByFingerPrintDescCommand { get; set; }

        public ICommand SwitchLibraryCommand { get; set; }

        public ICommand ToggleDisplayAuthorPaneCommand { get; set; }

        public ICommand ToggleDisplayInformationPaneCommand { get; set; }

        public ICommand ToggleDisplayTagPaneCommand { get; set; }

        public ICommand UnencryptionStartingCommand { get; set; }

        public ICommand UpdateBookByteSizeAllCommand { get; set; }

        public ICommand UpdateBookByteSizeStillNullCommand { get; set; }

        public ICommand UpdateBookTagCommand { get; set; }

        public ICommand UpdateBookFingerPrintAllCommand { get; set; }

        public ICommand UpdateBookFingerPrintStillNullCommand { get; set; }

        public ICommand UpdateApplicationCommand { get; set; }

        #endregion //コマンド

        #region コマンド登録

        private void RegisterCommands()
        {
            AboutSunctumCommand = new DelegateCommand(() =>
            {
                OpenVersionDialog();
            });
            ClearSearchResultCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.ClearSearchResult();
            });
            DisplaySideBySideCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.DisplayType = Domain.Logic.DisplayType.DisplayType.SideBySide;
            });
            RemoveHistoryCommand = new DelegateCommand<RecentOpenedLibrary>(p =>
            {
                var dao = DataAccessManager.AppDao.Build<RecentOpenedLibraryDao>();
                dao.Delete(new Dictionary<string, object>() { { "Path", p.Path } });
                LibraryVM.RecentOpenedLibraryList.Remove(p);
            });
            DetailsCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.DisplayType = Domain.Logic.DisplayType.DisplayType.Details;
            });
            EncryptionStartingCommand = new DelegateCommand(async () =>
            {
                await OpenEncryptionStartingDialog().ConfigureAwait(false);
            });
            EncryptionContinuingCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.StartEncryption(Configuration.ApplicationConfiguration.Password).ConfigureAwait(false);
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
                await OpenImportFileDialogThenImport().ConfigureAwait(false);
            });
            ImportFoldersCommand = new DelegateCommand(async () =>
            {
                await OpenImportFolderDialogThenImport().ConfigureAwait(false);
            });
            ImportLibraryCommand = new DelegateCommand(async () =>
            {
                await OpenImportLibraryDialog().ConfigureAwait(false);
            });
            LoadedCommand = new DelegateCommand(() =>
            {
                LoadLayout();
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
                IDialogResult result = new Prism.Services.Dialogs.DialogResult();
                DialogService.ShowDialog(nameof(PowerSearch), new DialogParameters() { { "Storage", ActiveDocumentViewModel.BookCabinet } }, ret => result = ret);
            });
            OpenStatisticsDialogCommand = new DelegateCommand(() =>
            {
                DialogService.ShowDialog(nameof(Views.Statistics));
            });
            OpenSwitchLibraryCommand = new DelegateCommand(async () =>
            {
                bool changed = OpenSwitchLibraryDialogAndChangeWorkingDirectory();
                if (changed)
                {
                    Terminate();
                    CloseAllTab();
                    Initialize1stPhase(false);
                    if (!Initialize2ndPhase(true))
                    {
                        return;
                    }
                    await Initialize3rdPhase().ConfigureAwait(false);
                }
            });
            OpenSearchPaneCommand = new DelegateCommand(() =>
            {
                this.ActiveDocumentViewModel.SearchPaneIsVisible = true;
            });
            OpenTagManagementDialogCommand = new DelegateCommand(() =>
            {
                OpenTagManagementDialog();
            });
            ReloadLibraryCommand = new DelegateCommand(async () =>
            {
                Terminate();
                CloseAllTab();
                Initialize1stPhase(false);
                await Initialize3rdPhase().ConfigureAwait(false);
            });
            ReplaceContentCommand = new DelegateCommand<System.Windows.DragEventArgs>(e =>
            {
                var a = e.Source as FrameworkElement;
                var b = e.Source as FrameworkContentElement;
                PageViewModel page = null;
                if (e.Source is FrameworkElement fe)
                {
                    page = fe.DataContext as PageViewModel;
                }
                else if (e.Source is FrameworkContentElement fce)
                {
                    page = fce.DataContext as PageViewModel;
                }
                var copyFrom = ((string[])e.Data.GetData(System.Windows.DataFormats.FileDrop)).First();
                var copyTo = page.Image.AbsoluteMasterPath;
                if (!Path.GetFileName(copyFrom).Equals(Path.GetFileName(copyTo)))
                {
                    System.Windows.MessageBox.Show("ファイル名が一致しないため, 置換できません。", "置換不可", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (page.Image.IsEncrypted)
                {
                    var fileMgr = new TxFileManager();
                    using (var scope = new TransactionScope())
                    using (var dataOpUnit = new DataOperationUnit())
                    {
                        dataOpUnit.Open(ConnectionManager.DefaultConnection);
                        fileMgr.Copy(copyFrom, copyTo, true);
                        EncryptImageDao dao = new EncryptImageDao();
                        var items = dao.FindBy(new Dictionary<string, object>() { { "TargetImageID", page.Image.ID } });
                        items.ToList().ForEach(x =>
                        {
                            dao.DeleteWhereIDIs(x.ID);
                        });
                        Encryptor.Encrypt(page.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", Configuration.ApplicationConfiguration.Password, dataOpUnit, fileMgr).Wait();
                        fileMgr.Delete(copyTo);
                        scope.Complete();
                    }
                }
                else
                {
                    File.Copy(copyFrom, copyTo);
                }
                System.Windows.MessageBox.Show("ファイルを置換しました。", "置換成功", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            ShowPreferenceDialogCommand = new DelegateCommand(() =>
            {
                ShowPreferenceDialog();
            });
            ShowDuplicateBooksCommand = new DelegateCommand(() =>
            {
                this.NewSearchTab(new ObservableCollection<BookViewModel>(BookFacade.FindDuplicateFingerPrint()));
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
            SortBookByFingerPrintAscCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByFingerPrintAsc;
            });
            SortBookByFingerPrintDescCommand = new DelegateCommand(() =>
            {
                ActiveDocumentViewModel.BookCabinet.Sorting = BookSorting.ByFingerPrintDesc;
            });
            SwitchLibraryCommand = new DelegateCommand<RecentOpenedLibrary>(async (p) =>
            {
                Terminate();
                await LibraryVM.Reset().ConfigureAwait(false);
                Configuration.ApplicationConfiguration.WorkingDirectory = p.Path;
                Configuration.Save(Configuration.ApplicationConfiguration);
                Initialize1stPhase(false);
                await Initialize3rdPhase().ConfigureAwait(false);
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
                await OpenUnencryptingDialog().ConfigureAwait(false);
            });
            UpdateBookByteSizeAllCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeAll().ConfigureAwait(false);
            });
            UpdateBookByteSizeStillNullCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookByteSizeStillNull().ConfigureAwait(false);
            });
            UpdateBookTagCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookTag().ConfigureAwait(false);
            });
            UpdateBookFingerPrintAllCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookFingerPrintAll().ConfigureAwait(false);
            });
            UpdateBookFingerPrintStillNullCommand = new DelegateCommand(async () =>
            {
                await LibraryVM.UpdateBookFingerPrintStillNull().ConfigureAwait(false);
            });
            UpdateApplicationCommand = new DelegateCommand(() =>
            {
                Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "boilersUpdater", "boilersUpdater.exe"));
            });
        }

        #endregion //コマンド登録

        #region プロパティ

        public System.Version SunctumVersion
        {
            [DebuggerStepThrough]
            get
            { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public IDialogService DialogService { get; }

        [Dependency]
        public ILibrary LibraryVM
        {
            [DebuggerStepThrough]
            get
            { return _LibraryVM; }
            set { SetProperty(ref _LibraryVM, value); }
        }

        [Dependency]
        public IEnumerable<Lazy<IAddMenuPlugin>> Plugins { get; set; }

        [Dependency]
        public IDataAccessManager DataAccessManager { get; set; }

        [Dependency]
        public IAuthorPaneViewModel AuthorPaneViewModel { get; set; }

        [Dependency]
        public ITagPaneViewModel TagPaneViewModel { get; set; }

        [Dependency]
        public IInformationPaneViewModel InformationPaneViewModel { get; set; }

        [Dependency]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        [Dependency]
        public ITagManager TagManager { get; set; }

        [Dependency]
        public IAuthorManager AuthorManager { get; set; }

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

        public IDocumentViewModelBase OldActiveDocumentViewModel
        {
            get { return _OldActiveDocumentViewModel; }
            set
            {
                SetProperty(ref _OldActiveDocumentViewModel, value);
            }
        }

        public IDocumentViewModelBase ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set
            {
                OldActiveDocumentViewModel = _ActiveDocumentViewModel;
                SetProperty(ref _ActiveDocumentViewModel, value);
                NotifyActiveTabChanged();
            }
        }

        public LayoutAnchorable AuthorPane { get; set; }

        public LayoutAnchorable TagPane { get; set; }

        public LayoutAnchorable InformationPane { get; set; }

        public Configuration Configuration { get { return Configuration.ApplicationConfiguration; } }

        public ReactivePropertySlim<TaskbarItemInfo> TaskbarItemInfo { get; set; } = new ReactivePropertySlim<TaskbarItemInfo>(new System.Windows.Shell.TaskbarItemInfo());

        #endregion

        #region 一般

        public void Initialize1stPhase(bool starting)
        {
            if (starting)
            {
                LibraryVM.ProgressManager.PropertyChanged += ProgressManager_PropertyChanged;

                if (App.Current is not null)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TagPaneViewModel.BuildContextMenus_Tags();
                        AuthorPaneViewModel.BuildContextMenus_Authors();
                    });
                }
            }

            WindowLeft = Configuration.ApplicationConfiguration.WindowRect.X;
            WindowTop = Configuration.ApplicationConfiguration.WindowRect.Y;
            WindowWidth = Configuration.ApplicationConfiguration.WindowRect.Width;
            WindowHeight = Configuration.ApplicationConfiguration.WindowRect.Height;
        }

        public bool Initialize2ndPhase(bool isSwitching, bool shiftPressed = false)
        {
            if (shiftPressed)
            {
                if (!OpenSwitchLibraryDialogAndChangeWorkingDirectory())
                {
                    Close();
                    return false;
                }
            }
            else if (!isSwitching)
            {
                IDialogResult dialogResult = null;
                App.Current.Dispatcher.Invoke(() =>
                {
                    DialogService.ShowDialog("Top", ret => dialogResult = ret);
                });
                if (dialogResult.Result == ButtonResult.Cancel || dialogResult.Result == ButtonResult.None)
                {
                    Close();
                    return false;
                }
                Configuration.ApplicationConfiguration.WorkingDirectory = dialogResult.Parameters.GetValue<string>("WorkingDirectory");
                Configuration.Save(Configuration.ApplicationConfiguration);
            }

            CloseAllTab();
            return true;
        }

        public async Task Initialize3rdPhase()
        {
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
            InitializeWindowComponent();
            await ManageVcDB();
            await ManageAppDB();
            await InitializeVcGitHubReleasesLatest();
            await IncrementNumberOfBoots();
            await RecordVersionControlIfFirstLaunch();

            Configuration.ApplicationConfiguration.ConnectionString = Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory);
            Configuration.ApplicationConfiguration.ConnectionString = Configuration.ApplicationConfiguration.ConnectionString.Replace("\\", "\\\\");
            s_logger.Info($"Set ConnectionString={Configuration.ApplicationConfiguration.ConnectionString}");
            ConnectionManager.SetDefaultConnection(Guid.Parse("9056E8CF-745D-4BCC-AEB9-14B1D1B40F37"), Configuration.ApplicationConfiguration.ConnectionString, typeof(SQLiteConnection));

            try
            {
                await LibraryVM.Initialize().ConfigureAwait(false);
                await LibraryVM.UnlockIfLocked().ConfigureAwait(false);
                await LibraryVM.Reset().ConfigureAwait(false);
                await LibraryVM.Load()
                    .ContinueWith(_ =>
                    {
                        HomeDocumentViewModel.BookCabinet = LibraryVM.CreateBookStorage();
                        HomeDocumentViewModel.BookCabinet.ClearSearchResult();

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

                        var displayType = Configuration.ApplicationConfiguration.DisplayType;
                        if (displayType != null)
                        {
                            HomeDocumentViewModel.BookCabinet.DisplayType = DisplayType.GetReferenceByName(displayType);
                        }

                        ((DocumentViewModelBase)HomeDocumentViewModel).IsVisible = true;
                        ((DocumentViewModelBase)HomeDocumentViewModel).IsSelected = true;

                        SetEvent();

                        NotifyActiveTabChanged();
                    }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Close();
            }
        }

        private async Task InitializeVcGitHubReleasesLatest()
        {
            var dao = DataAccessManager.VcDao.Build<GitHubReleasesLatestDao>();
            var recordsCount = await dao.CountAllAsync();
            if (recordsCount == 0)
            {
                var newrecord = new GitHubReleasesLatest()
                {
                    URL = "https://api.github.com/repos/dhq-boiler/Sunctum/releases/latest",
                };
                await dao.InsertAsync(newrecord);
            }
        }

        private async Task RecordVersionControlIfFirstLaunch()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var version = assembly.Version;
            string key = GetVersionString(version);
            var dao = DataAccessManager.VcDao.Build<VersionControlDao>();
            var records = await dao.FindByAsync(new Dictionary<string, object>() { { "FullVersion", key } }).ToListAsync();
            if (records.Count() == 0)
            {
                var newrecord = new VersionControl()
                {
                    FullVersion = key,
                    Major = version.Major,
                    Minor = version.Minor,
                    Build = version.Build,
                    Revision = version.Revision,
                    IsValid = true,
                    InstalledDate = DateTime.Now,
                    RetiredDate = null,
                };
                await dao.InsertAsync(newrecord);
            }
        }

        private static string GetVersionString(System.Version version)
        {
            var key = $"v{version.Major}";
            if (version.Revision > 0)
            {
                key += $".{version.Minor}.{version.Build}.{version.Revision}";
            }
            else if (version.Build > 0)
            {
                key += $".{version.Minor}.{version.Build}";
            }
            else if (version.Minor > 0)
            {
                key += $".{version.Minor}";
            }

            return key;
        }

        private async Task IncrementNumberOfBoots()
        {
            var id = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var dao = DataAccessManager.AppDao.Build<StatisticsDao>();
            var statistics = await dao.FindByAsync(new Dictionary<string, object>() { { "ID", id } }).ToListAsync();
            if (statistics.Count() == 0)
            {
                var newStatistics = new Domain.Models.Statistics();
                newStatistics.ID = id;
                newStatistics.NumberOfBoots = 1;
                await dao.InsertAsync(newStatistics);
            }
            else
            {
                var existStatistics = statistics.First();
                existStatistics.NumberOfBoots += 1;
                await dao.UpdateAsync(existStatistics);
            }
        }

        public void NewSearchTab(ObservableCollection<BookViewModel> onStage)
        {
            var newTabViewModel = (App.Current.Resources["Ioc"] as IUnityContainer).Resolve<IDocumentViewModelBase>("SearchDocumentViewModel");
            newTabViewModel.Title = "Search results";
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource.Clear();
            newTabViewModel.BookCabinet.BookSource.AddRange(onStage);
            newTabViewModel.BookCabinet.Sorting = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.Sorting;
            newTabViewModel.BookCabinet.DisplayType = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.DisplayType;
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
            var newTabViewModel = (App.Current.Resources["Ioc"] as IUnityContainer).Resolve<IDocumentViewModelBase>("ContentDocumentViewModel");
            newTabViewModel.Title = bookViewModel.Title;
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource.Clear();
            newTabViewModel.BookCabinet.BookSource.Add(bookViewModel);
            newTabViewModel.BookCabinet.Sorting = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.Sorting;
            newTabViewModel.BookCabinet.DisplayType = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.DisplayType;
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
            var newTabViewModel = (App.Current.Resources["Ioc"] as IUnityContainer).Resolve<IDocumentViewModelBase>("ContentDocumentViewModel");
            newTabViewModel.Title = "Filtered";
            newTabViewModel.BookCabinet = LibraryVM.CreateBookStorage();
            newTabViewModel.BookCabinet.BookSource.Clear();
            newTabViewModel.BookCabinet.BookSource.AddRange(list);
            newTabViewModel.BookCabinet.Sorting = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.Sorting;
            newTabViewModel.BookCabinet.DisplayType = (App.Current.MainWindow.DataContext as MainWindowViewModel).ActiveDocumentViewModel.BookCabinet.DisplayType;
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

            if (App.Current is not null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    DockingDocumentViewModels.Clear();
                });
            }
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

        public async Task ManageAppDB()
        {
            DataVersionManager dvManager = new DataVersionManager();
            dvManager.CurrentConnection = DataAccessManager.AppDao.CurrentConnection;
            dvManager.RegisterChangePlan(new ChangePlan_AppDb_VersionOrigin(VersioningMode.ByTick));
            dvManager.RegisterChangePlan(new ChangePlan_AppDb_Version_1(VersioningMode.ByTick));
            dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo;

            await dvManager.UpgradeToTargetVersion();
        }

        public async Task ManageVcDB()
        {
            DataVersionManager dvManager = new DataVersionManager();
            dvManager.CurrentConnection = DataAccessManager.VcDao.CurrentConnection;
            dvManager.RegisterChangePlan(new ChangePlan_VC_VersionOrigin(VersioningMode.ByTick));
            dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo_VC;

            await dvManager.UpgradeToTargetVersion();
        }

        private void DvManager_FinishedToUpgradeTo_VC(object sender, ModifiedEventArgs e)
        {
            s_logger.Info($"Heavy Modifying VcDB Count : {e.ModifiedCount}");

            if (e.ModifiedCount > 0)
            {
                SQLiteBaseDao<Dummy>.Vacuum(DataAccessManager.VcDao.CurrentConnection);
            }
        }

        private void DvManager_FinishedToUpgradeTo(object sender, ModifiedEventArgs e)
        {
            s_logger.Info($"Heavy Modifying AppDB Count : {e.ModifiedCount}");

            if (e.ModifiedCount > 0)
            {
                SQLiteBaseDao<Dummy>.Vacuum(DataAccessManager.AppDao.CurrentConnection);
            }
        }

        private void SetEvent()
        {
            HomeDocumentViewModel.BookCabinet.SearchCleared += LibraryVM_SearchCleared;
            HomeDocumentViewModel.BookCabinet.Searched += LibraryVM_Searched;
        }

        public void InitializeWindowComponent()
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

            HomeDocumentViewModel.CloseSearchPane();
            HomeDocumentViewModel.CloseImage();
            HomeDocumentViewModel.CloseBook();
            HomeDocumentViewModel.ResetScrollOffsetPool();
            //HomeDocumentViewModel.ResetScrollOffset();
            LibraryVM.ProgressManager.Complete();
            TooltipOnProgressBar = "Ready";
            HomeDocumentViewModel.ClearSelectedItems();
            AuthorPaneViewModel.ClearSelectedItems();
            TagPaneViewModel.ClearSelectedItems();
        }

        private void LoadLayout()
        {
            var dockingManager = App.Current.MainWindow.GetChildOfType<DockingManager>();
            if (!File.Exists(Specifications.APP_LAYOUT_CONFIG_FILENAME))
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                serializer.Serialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
            }

            var deserializer = new XmlLayoutSerializer(dockingManager);
            deserializer.LayoutSerializationCallback += Deserializer_LayoutSerializationCallback;
            deserializer.Deserialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
        }

        private void Deserializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            switch (e.Model.ContentId)
            {
                case "home":
                    break;
                case "author":
                    AuthorPane = (LayoutAnchorable)e.Model;
                    break;
                case "tag":
                    TagPane = (LayoutAnchorable)e.Model;
                    break;
                case "information":
                    InformationPane = (LayoutAnchorable)e.Model;
                    break;
            }
        }

        private void SetMainWindowTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionStr = GetVersionString(version);
            var appnameAndVersion = $"Sunctum {versionStr}";

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
            var dialog = new FolderBrowserDialog();
            dialog.UseDescriptionForTitle = true;
            dialog.Description = "ライブラリディレクトリの場所";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Configuration.ApplicationConfiguration.WorkingDirectory = dialog.SelectedPath;
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

        /// <summary>
        /// Terminateメソッドから呼び出しても機能しないので注意。
        /// </summary>
        public void SaveLayout()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (App.Current is null)
                    return;
                if (App.Current.MainWindow is null)
                    return;
                var dockingManager = App.Current.MainWindow.GetChildOfType<DockingManager>();
                if (dockingManager is null)
                    return;
                var serializer = new XmlLayoutSerializer(dockingManager);
                serializer.Serialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
            });
        }

        public void Terminate()
        {
            OnmemoryImageManager.Instance.Clear();

            var config = Configuration.ApplicationConfiguration;
            if (HomeDocumentViewModel.BookCabinet != null)
                config.BookSorting = BookSorting.GetPropertyName(HomeDocumentViewModel.BookCabinet.Sorting);
            config.DisplayAuthorPane = DisplayAuthorPane;
            config.DisplayInformationPane = DisplayInformationPane;
            config.DisplayTagPane = DisplayTagPane;
            config.AuthorSorting = AuthorSorting.GetPropertyName(AuthorManager.Sorting);
            config.TagSorting = ImageTagCountSorting.GetPropertyName(TagManager.Sorting);
            if (HomeDocumentViewModel.BookCabinet != null)
                config.DisplayType = DisplayType.GetPropertyName(HomeDocumentViewModel.BookCabinet.DisplayType);

            if (config.StoreWindowPosition)
            {
                config.WindowRect = new Domain.Models.Rect(WindowLeft, WindowTop, WindowWidth, WindowHeight);
            }
            Configuration.ApplicationConfiguration.Password = null;
            Configuration.Save(config);

            _disposable.Clear();
        }

        public void Close()
        {
            SaveLayout();
            ConnectionManager.DisposeDebris(Guid.Parse("9056E8CF-745D-4BCC-AEB9-14B1D1B40F37"));
            if (_disposable is not null && !_disposable.IsDisposed)
            {
                _disposable.Dispose();
            }
            if (System.Windows.Application.Current is not null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.Shutdown();
                });
            }
        }

        public void ChangeActiveContent()
        {
            if (OldActiveDocumentViewModel == null || OldActiveDocumentViewModel.BookCabinet == null) return;
            var oldDisplayType = OldActiveDocumentViewModel.BookCabinet.DisplayType;
            var oldSort = OldActiveDocumentViewModel.BookCabinet.Sorting;
            ActiveDocumentViewModel.BookCabinet.DisplayType = oldDisplayType;
            ActiveDocumentViewModel.BookCabinet.Sorting = oldSort;
        }

        #endregion //一般

        private void OpenVersionDialog()
        {
            DialogService.ShowDialog(nameof(Views.Version));
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
            DialogService.ShowDialog(nameof(Preferences));
        }

        private async Task OpenImportLibraryDialog()
        {
            var dialog = new FolderSelectDialog
            {
                Title = "ライブラリディレクトリの場所"
            };

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.ImportLibrary(dialog.FileName).ConfigureAwait(false);
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
                await LibraryVM.ImportAsync(dialog.FileNames.ToArray()).ConfigureAwait(false);
            }
        }

        private async Task OpenImportFolderDialogThenImport()
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await LibraryVM.ImportAsync(new string[] { dialog.SelectedPath }).ConfigureAwait(false);
            }
        }

        private async Task OpenEncryptionStartingDialog()
        {
            var dialog = new EncryptionStartingDialog();

            if (dialog.ShowDialog() == true)
            {
                await LibraryVM.StartEncryption(dialog.Password).ConfigureAwait(false);
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
