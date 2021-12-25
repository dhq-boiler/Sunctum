

using Homura.ORM;
using NLog;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using Prism.Unity;
using Sunctum.Converters;
using Sunctum.Core.Extensions;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Managers;
using Sunctum.ViewModels;
using Sunctum.Views;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Unity;

namespace Sunctum
{
    public partial class App : PrismApplication
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            s_logger.Info($"Sunctum Personal Photo Library {version}");
            s_logger.Info("Copyright (C) dhq_boiler 2015-2021. All rights reserved.");
            s_logger.Info("SUNCTUM IS LAUNCHING");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = $"{Directory.GetCurrentDirectory()}\\plugins\\" };
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Configuration.ApplicationConfiguration = Configuration.Load();

            containerRegistry.GetContainer().AddExtension(new Diagnostic()).AddExtension(new LogResolvesUnityContainerExtension());
            containerRegistry.RegisterSingleton<IMainWindowViewModel, MainWindowViewModel>();
            containerRegistry.RegisterSingleton<IAuthorPaneViewModel, AuthorPaneViewModel>();
            containerRegistry.RegisterSingleton<ITagPaneViewModel, TagPaneViewModel>();
            containerRegistry.RegisterSingleton<IInformationPaneViewModel, InformationPaneViewModel>();
            containerRegistry.RegisterSingleton<ILibrary, Library>();
            containerRegistry.RegisterSingleton<ITagManager, TagManager>();
            containerRegistry.RegisterSingleton<IAuthorManager, AuthorManager>();
            containerRegistry.RegisterSingleton<IProgressManager, ProgressManager>();
            containerRegistry.RegisterSingleton<ITaskManager, TaskManager>();
            containerRegistry.RegisterSingleton<IBookExporting, BookExporting>();
            containerRegistry.RegisterSingleton<IBookImporting, BookImporting>();
            containerRegistry.RegisterSingleton<IBookRemoving, BookRemoving>();
            containerRegistry.RegisterSingleton<IByteSizeCalculating, ByteSizeCalculating>();
            containerRegistry.RegisterSingleton<ILibraryInitializing, LibraryInitializing>();
            containerRegistry.RegisterSingleton<IImageTagAdding, ImageTagAdding>();
            containerRegistry.RegisterSingleton<IImageTagRemoving, ImageTagRemoving>();
            containerRegistry.RegisterSingleton<ITagRemoving, TagRemoving>();
            containerRegistry.RegisterSingleton<ILibraryImporting, LibraryImporting>();
            containerRegistry.RegisterSingleton<IPageRemoving, PageRemoving>();
            containerRegistry.RegisterSingleton<IPageScrapping, PageScrapping>();
            containerRegistry.RegisterSingleton<IBookThumbnailRemaking, BookThumbnailRemaking>();
            containerRegistry.RegisterSingleton<IPageThumbnailRemaking, PageThumbnailRemaking>();
            containerRegistry.RegisterSingleton<ILibraryLoading, LibraryLoading>();
            containerRegistry.RegisterSingleton<IPageOrderUpdating, PageOrderUpdating>();
            containerRegistry.RegisterSingleton<IRecentOpenedLibraryUpdating, RecentOpenedLibraryUpdating>();
            containerRegistry.RegisterSingleton<IDirectoryNameParserManager, DirectoryNameParserManager>();
            containerRegistry.RegisterSingleton<ILibraryResetting, LibraryResetting>();
            containerRegistry.RegisterSingleton<IBookLoading, BookLoading>();
            containerRegistry.RegisterSingleton<IBookTagInitializing, BookTagInitializing>();
            containerRegistry.RegisterSingleton<IEncryptionStarting, EncryptionStarting>();
            containerRegistry.RegisterSingleton<IUnencryptionStarting, UnencryptionStarting>();
            containerRegistry.RegisterSingleton<IBookHashing, BookHashing>();
            containerRegistry.RegisterSingleton<IValueConverter, BookSortingToBool>("BookSortingToBool");
            containerRegistry.RegisterSingleton<IValueConverter, DisplayTypeToBool>("DisplayTypeToBool");
            containerRegistry.RegisterSingleton<IValueConverter, TagSortingToBool>("TagSortingToBool");
            containerRegistry.RegisterSingleton<IValueConverter, AuthorSortingToBool>("AuthorSortingToBool");
            containerRegistry.RegisterSingleton<IDataAccessManager, DataAccessManager>();
            containerRegistry.RegisterInstance<IDaoBuilder>(new DaoBuilder(new Connection(ConnectionStringBuilder.Build(Specifications.APP_DB_FILENAME), typeof(SQLiteConnection))), "AppDao");
            containerRegistry.RegisterInstance<IDaoBuilder>(new DaoBuilder(new Connection(Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory), typeof(SQLiteConnection))), "WorkingDao");

            containerRegistry.RegisterDialog<ChangeStar, ChangeStarViewModel>();
            containerRegistry.RegisterDialog<BookProperty, BookPropertyDialogViewModel>();
            containerRegistry.RegisterDialog<PowerSearch, PowerSearchViewModel>();
            containerRegistry.RegisterDialog<Views.Statistics, StatisticsDialogViewModel>();
            containerRegistry.RegisterDialog<Views.Version, VersionViewModel>();
            containerRegistry.RegisterDialog<Views.Preferences, PreferencesDialogViewModel>();
            containerRegistry.RegisterDialog<Views.Export, ExportDialogViewModel>();
            containerRegistry.RegisterDialog<Views.ErrorReport, ErrorReportDialogViewModel>();

            containerRegistry.RegisterSingleton<ISelectManager, SelectManager>();
            containerRegistry.RegisterSingleton<IHomeDocumentViewModel, HomeDocumentViewModel>();
            containerRegistry.Register<IDocumentViewModelBase, SearchDocumentViewModel>("SearchDocumentViewModel");
            containerRegistry.Register<IDocumentViewModelBase, ContentDocumentViewModel>("ContentDocumentViewModel");

            App.Current.Resources.Add("Ioc", containerRegistry.GetContainer());
        }

        /// <summary>
        /// WPF UIスレッドでの未処理例外スロー時のイベントハンドラ
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            this.ReportUnhandledException(e.Exception);
        }

        /// <summary>
        /// UIスレッド以外の未処理例外スロー時のイベントハンドラ
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.ReportUnhandledException(e.ExceptionObject as Exception);
        }

        private void ReportUnhandledException(Exception exception)
        {
            s_logger.Error(exception);
            var dialogService = Container.Resolve<IDialogService>();
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("Exception", exception);
            IDialogResult dialogResult = new DialogResult();
            dialogService.ShowDialog(nameof(ErrorReport), parameters, ret => dialogResult = ret);
            if (dialogResult.Result == ButtonResult.None)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// アプリケーション終了時のイベントハンドラ
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            s_logger.Info("SUNCTUM IS SHUTTING DOWN");

            this.DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            base.OnExit(e);
        }
    }
}
