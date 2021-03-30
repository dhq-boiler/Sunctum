

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public interface ILibrary : IBookStorage
    {
        ObservableCollection<RecentOpenedLibrary> RecentOpenedLibraryList { get; set; }

        Task Load();

        Task RemovePages(PageViewModel[] pages);

        Task RemoveBooks(BookViewModel[] books);

        Task ExportBooks(BookViewModel[] books, string directory, bool tag);

        Task ScrapPages(AuthorViewModel author, string title, PageViewModel[] pages);

        Task RemakeThumbnail(IEnumerable<BookViewModel> books);

        Task RemakeThumbnail(IEnumerable<PageViewModel> pages);

        Task ImportAsync(string[] objectPaths);

        Task ImportLibrary(string directory);

        Task StartEncryption(string password);

        BookViewModel OrderForward(PageViewModel page, BookViewModel book);

        BookViewModel OrderBackward(PageViewModel page, BookViewModel book);

        Task SaveBookContentsOrder(BookViewModel book);

        bool IsDirty(BookViewModel book);

        ITagManager TagMng { get; }

        IProgressManager ProgressManager { get; }

        IAuthorManager AuthorManager { get; }

        ITaskManager TaskManager { get; }

        Task UpdateBookByteSizeAll();

        Task UpdateBookByteSizeStillNull();

        Task Initialize();

        Task Reset();

        IArrangedBookStorage CreateBookStorage();
        Task UpdateBookTag();
        bool UnlockIfLocked();
        Task StartUnencryption(string password);
        Task UpdateBookFingerPrintAll();
        Task UpdateBookFingerPrintStillNull();
    }

    public delegate void SearchedEventHandler(object sender, SearchedEventArgs e);
}
