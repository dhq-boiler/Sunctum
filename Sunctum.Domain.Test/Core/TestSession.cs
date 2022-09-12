using Homura.ORM;
using NUnit.Framework;
using Prism.Ioc;
using Sunctum.Converters;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Managers;
using Sunctum.ViewModels;
using System.Data.SQLite;
using System.Windows.Data;
using Unity;

namespace Sunctum.Domain.Test.Core
{
    public abstract class TestSession
    {
        public UnityContainer Container { get; private set; }

        public abstract string GetTestDirectory();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var config = new Configuration();
            config.WorkingDirectory = GetTestDirectory();
            Configuration.ApplicationConfiguration = config;

            Container = new UnityContainer();
            Container.RegisterType<IMainWindowViewModel, MainWindowViewModel>();
            Container.RegisterType<IHomeDocumentViewModel, HomeDocumentViewModel>();
            Container.RegisterType<IAuthorPaneViewModel, AuthorPaneViewModel>();
            Container.RegisterType<ITagPaneViewModel, TagPaneViewModel>();
            Container.RegisterType<IInformationPaneViewModel, InformationPaneViewModel>();
            Container.RegisterSingleton<ILibrary, Library>();
            Container.RegisterType<ITagManager, TagManager>();
            Container.RegisterType<IAuthorManager, AuthorManager>();
            Container.RegisterType<IProgressManager, ProgressManager>();
            Container.RegisterType<ITaskManager, TaskManager>();
            Container.RegisterType<IBookExporting, BookExporting>();
            Container.RegisterType<IBookImporting, BookImporting>();
            Container.RegisterType<IBookRemoving, BookRemoving>();
            Container.RegisterType<IByteSizeCalculating, ByteSizeCalculating>();
            Container.RegisterType<ILibraryInitializing, LibraryInitializing>();
            Container.RegisterType<IImageTagAdding, ImageTagAdding>();
            Container.RegisterType<IImageTagRemoving, ImageTagRemoving>();
            Container.RegisterType<ITagRemoving, TagRemoving>();
            Container.RegisterType<ILibraryImporting, LibraryImporting>();
            Container.RegisterType<IPageRemoving, PageRemoving>();
            Container.RegisterType<IPageScrapping, PageScrapping>();
            Container.RegisterType<IBookThumbnailRemaking, BookThumbnailRemaking>();
            Container.RegisterType<IPageThumbnailRemaking, PageThumbnailRemaking>();
            Container.RegisterType<ILibraryLoading, LibraryLoading>();
            Container.RegisterType<IPageOrderUpdating, PageOrderUpdating>();
            Container.RegisterType<IRecentOpenedLibraryUpdating, RecentOpenedLibraryUpdating>();
            Container.RegisterType<IDirectoryNameParserManager, DirectoryNameParserManager>();
            Container.RegisterType<ILibraryResetting, LibraryResetting>();
            Container.RegisterType<IBookLoading, BookLoading>();
            Container.RegisterType<IBookTagInitializing, BookTagInitializing>();
            Container.RegisterType<IEncryptionStarting, EncryptionStarting>();
            Container.RegisterType<IUnencryptionStarting, UnencryptionStarting>();
            Container.RegisterType<IBookHashing, BookHashing>();
            Container.RegisterType<IValueConverter, BookSortingToBool>("BookSortingToBool");
            Container.RegisterType<IValueConverter, DisplayTypeToBool>("DisplayTypeToBool");
            Container.RegisterType<IValueConverter, TagSortingToBool>("TagSortingToBool");
            Container.RegisterType<IValueConverter, AuthorSortingToBool>("AuthorSortingToBool");
            Container.RegisterType<IDataAccessManager, DataAccessManager>();
            Container.RegisterInstance<IDaoBuilder>("AppDao", new DaoBuilder(new Connection(ConnectionStringBuilder.Build(Specifications.APP_DB_FILENAME), typeof(SQLiteConnection))));
            Container.RegisterInstance<IDaoBuilder>("WorkingDao", new DaoBuilder(new Connection(Specifications.GenerateConnectionString(Configuration.ApplicationConfiguration.WorkingDirectory), typeof(SQLiteConnection))));
            Container.RegisterInstance<IDaoBuilder>("VcDao", new DaoBuilder(new Connection(ConnectionStringBuilder.Build(Specifications.VC_DB_FILENAME), typeof(SQLiteConnection))));
        }
    }
}
