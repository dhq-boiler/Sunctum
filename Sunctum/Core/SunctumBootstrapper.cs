

using Ninject;
using Prism.Ninject;
using Sunctum.Converters;
using Sunctum.Core.Extensions;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Managers;
using Sunctum.ViewModels;
using Sunctum.Views;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Sunctum.Core
{
    internal class SunctumBootstrapper : NinjectBootstrapper
    {
        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            Kernel.Bind<IMainWindow>().To<MainWindow>().InSingletonScope();
            Kernel.Bind<IMainWindowViewModel>().To<MainWindowViewModel>().InSingletonScope();
            Kernel.Bind<ILibraryManager>().To<LibraryManager>().InSingletonScope();
            Kernel.Bind<ITagManager>().To<TagManager>().InSingletonScope();
            Kernel.Bind<IAuthorManager>().To<AuthorManager>().InSingletonScope();
            Kernel.Bind<IProgressManager>().To<ProgressManager>().InSingletonScope();
            Kernel.Bind<ITaskManager>().To<TaskManager>().InSingletonScope();
            Kernel.Bind<IDataAccessManager>().To<DataAccessManager>().InSingletonScope();

            Kernel.Bind<IBookExporting>().To<BookExporting>().InTransientScope();
            Kernel.Bind<IBookImporting>().To<BookImporting>().InTransientScope();
            Kernel.Bind<IBookRemoving>().To<BookRemoving>().InTransientScope();
            Kernel.Bind<IByteSizeCalculating>().To<ByteSizeCalculating>().InTransientScope();
            Kernel.Bind<ILibraryInitializing>().To<LibraryInitializing>().InTransientScope();
            Kernel.Bind<IImageTagAdding>().To<ImageTagAdding>().InTransientScope();
            Kernel.Bind<IImageTagRemoving>().To<ImageTagRemoving>().InTransientScope();
            Kernel.Bind<ITagRemoving>().To<TagRemoving>().InTransientScope();
            Kernel.Bind<ILibraryImporting>().To<LibraryImporting>().InTransientScope();
            Kernel.Bind<IPageRemoving>().To<PageRemoving>().InTransientScope();
            Kernel.Bind<IPageScrapping>().To<PageScrapping>().InTransientScope();
            Kernel.Bind<IBookThumbnailRemaking>().To<BookThumbnailRemaking>().InTransientScope();
            Kernel.Bind<IPageThumbnailRemaking>().To<PageThumbnailRemaking>().InTransientScope();
            Kernel.Bind<ILibraryLoading>().To<LibraryLoading>().InTransientScope();
            Kernel.Bind<IPageOrderUpdating>().To<PageOrderUpdating>().InTransientScope();
            Kernel.Bind<IRecentOpenedLibraryUpdating>().To<RecentOpenedLibraryUpdating>().InTransientScope();
            Kernel.Bind<IDirectoryNameParserManager>().To<DirectoryNameParserManager>().InSingletonScope();
            Kernel.Bind<ILibraryResetting>().To<LibraryResetting>().InSingletonScope();
            Kernel.Bind<IBookLoading>().To<BookLoading>().InSingletonScope();

            Kernel.Bind<IValueConverter>().To<BookSortingToBool>().InSingletonScope().Named("BookSortingToBool");

            Kernel.Get<IDataAccessManager>().AppDao = new DaoBuilder(new Connection(ConnectionStringBuilder.Build(Specifications.APP_DB_FILENAME), typeof(SQLiteConnection)));

            BookSortingToBool.Resolve = (type) => Kernel.Get(type);
            BookSortingToBool.ResolveNamed = (type, name) => Kernel.Get(type, name);

            //var config = Configuration.Load();
            //Kernel.Inject(config);
            Configuration.ApplicationConfiguration = Configuration.Load();

            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            foreach (var dllFile in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dllFile);
                Kernel.BindExportsInAssembly(assembly);
            }
        }

        protected override DependencyObject CreateShell()
        {
            return (DependencyObject)Kernel.Get<IMainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = Shell as Window;

            if (Configuration.ApplicationConfiguration.StoreWindowPosition)
            {
                var rect = Configuration.ApplicationConfiguration.WindowRect;
                if (rect != null && rect.Width != 0 && rect.Height != 0)
                {
                    Application.Current.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;

                    var mainWindowVM = Kernel.Get<IMainWindowViewModel>();

                    mainWindowVM.WindowLeft = rect.X;
                    mainWindowVM.WindowTop = rect.Y;
                    mainWindowVM.WindowWidth = rect.Width;
                    mainWindowVM.WindowHeight = rect.Height;
                }
            }
            Application.Current.MainWindow.Show();
        }
    }
}
