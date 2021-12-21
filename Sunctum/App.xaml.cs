

using Homura.ORM;
using NLog;
using Prism.Ioc;
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer().AddExtension(new Diagnostic()).AddExtension(new LogResolvesUnityContainerExtension());
            containerRegistry.RegisterSingleton<IMainWindowViewModel, MainWindowViewModel>();
            containerRegistry.RegisterSingleton<IHomeDocumentViewModel, HomeDocumentViewModel>();
            containerRegistry.RegisterSingleton<IAuthorPaneViewModel, AuthorPaneViewModel>();
            containerRegistry.RegisterSingleton<ITagPaneViewModel, TagPaneViewModel>();
            containerRegistry.RegisterSingleton<IInformationPaneViewModel, InformationPaneViewModel>();
            containerRegistry.RegisterSingleton<ILibrary, Library>();
            containerRegistry.RegisterSingleton<ITagManager, TagManager>();
            containerRegistry.RegisterSingleton<IAuthorManager, AuthorManager>();
            containerRegistry.RegisterSingleton<IProgressManager, ProgressManager>();
            containerRegistry.RegisterSingleton<ITaskManager, TaskManager>();
            containerRegistry.RegisterSingleton<IDataAccessManager, DataAccessManager>();
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
            containerRegistry.GetContainer().Resolve<IDataAccessManager>().AppDao = new DaoBuilder(new Connection(ConnectionStringBuilder.Build(Specifications.APP_DB_FILENAME), typeof(SQLiteConnection)));

            containerRegistry.RegisterDialog<ChangeStar, ChangeStarViewModel>();
            containerRegistry.RegisterDialog<BookProperty, BookPropertyDialogViewModel>();
            containerRegistry.RegisterDialog<PowerSearch, PowerSearchViewModel>();
            containerRegistry.RegisterDialog<Views.Statistics, StatisticsDialogViewModel>();

            DisplayTypeToBool.Resolve = (type) => containerRegistry.GetContainer().Resolve(type);
            DisplayTypeToBool.ResolveNamed = (type, name) => containerRegistry.GetContainer().Resolve(type, name);
            AuthorSortingToBool.Resolve = (type) => containerRegistry.GetContainer().Resolve(type);
            AuthorSortingToBool.ResolveNamed = (type, name) => containerRegistry.GetContainer().Resolve(type, name);
            TagSortingToBool.Resolve = (type) => containerRegistry.GetContainer().Resolve(type);
            TagSortingToBool.ResolveNamed = (type, name) => containerRegistry.GetContainer().Resolve(type, name);
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
            var dialog = new ErrorReportDialog(exception);
            dialog.ShowDialog();
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
