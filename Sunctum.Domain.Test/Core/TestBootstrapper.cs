

using Ninject;
using Prism.Ninject;
using Sunctum.Converters;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Logic.Async;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Managers;
using Sunctum.ViewModels;
using System.Data.SQLite;
using System.Windows.Data;

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
            Kernel.Bind<IMainWindowViewModel>().To<MainWindowViewModel>().InSingletonScope();

            Kernel.Bind<IHomeDocumentViewModel>().To<HomeDocumentViewModel>().InSingletonScope();
            Kernel.Bind<IAuthorPaneViewModel>().To<AuthorPaneViewModel>().InSingletonScope();
            Kernel.Bind<ITagPaneViewModel>().To<TagPaneViewModel>().InSingletonScope();
            Kernel.Bind<IInformationPaneViewModel>().To<InformationPaneViewModel>().InSingletonScope();

            Kernel.Bind<ILibrary>().To<Library>().InSingletonScope();
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
            Kernel.Bind<IBookTagInitializing>().To<BookTagInitializing>().InSingletonScope();

            Kernel.Bind<IValueConverter>().To<BookSortingToBool>().InSingletonScope().Named("BookSortingToBool");
            Kernel.Bind<IValueConverter>().To<TagSortingToBool>().InSingletonScope().Named("TagSortingToBool");
            Kernel.Bind<IValueConverter>().To<AuthorSortingToBool>().InSingletonScope().Named("AuthorSortingToBool");

            var daoBuilder = Kernel.Get<IDataAccessManager>();
            daoBuilder.WorkingDao = new DaoBuilder(new Connection(_libraryConnectionString, typeof(SQLiteConnection)));
        }

        internal T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
