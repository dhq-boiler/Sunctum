

using Ninject;
using Prism.Ninject;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Logic.Async;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Managers;
using System.Data.SQLite;

namespace Sunctum.Domain.Test.Core
{
    internal class TestBootstrapper : NinjectBootstrapper
    {
        private string _libraryConnectionString;

        public TestBootstrapper(string libraryConnectionString)
        {
            _libraryConnectionString = libraryConnectionString;
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            Kernel.Bind<ILibraryManager>().To<LibraryManager>().InSingletonScope();
            Kernel.Bind<IAuthorManager>().To<AuthorManager>().InSingletonScope();
            Kernel.Bind<ITagManager>().To<TagManager>().InSingletonScope();
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
            Kernel.Bind<IRecentOpenedLibraryUpdating>().To<RecentOpenedLibraryUpdatingMock>().InTransientScope();
            Kernel.Bind<IDirectoryNameParserManager>().To<DirectoryNameParserManager>().InSingletonScope();
            Kernel.Bind<ILibraryResetting>().To<LibraryResetting>().InSingletonScope();
            Kernel.Bind<IBookLoading>().To<BookLoading>().InSingletonScope();

            var daoBuilder = Kernel.Get<IDataAccessManager>();
            daoBuilder.WorkingDao = new DaoBuilder(new Connection(_libraryConnectionString, typeof(SQLiteConnection)));
        }

        internal T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
