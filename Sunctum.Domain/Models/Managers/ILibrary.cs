

using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public interface ILibrary
    {
        ObservableCollection<BookViewModel> BookSource { get; set; }

        ObservableCollection<BookViewModel> OnStage { get; }

        ObservableCollection<RecentOpenedLibrary> RecentOpenedLibraryList { get; set; }

        void FireFillContents(BookViewModel target);

        void RunFillContents(BookViewModel target);

        void FireFillContentsWithImage(BookViewModel target);

        void RunFillContentsWithImage(BookViewModel target);

        void AddToMemory(BookViewModel target);

        void RemoveFromMemory(BookViewModel target);

        void AccessDispatcherObject(Action accessAction);

        Task Load();

        Task RemovePages(PageViewModel[] pages);

        Task RemoveBooks(BookViewModel[] books);

        Task ExportBooks(BookViewModel[] books, string directory, bool tag);

        Task ScrapPages(AuthorViewModel author, string title, PageViewModel[] pages);

        Task RemakeThumbnail(IEnumerable<BookViewModel> books);

        Task RemakeThumbnail(IEnumerable<PageViewModel> pages);

        Task ImportAsync(string[] objectPaths);

        Task ImportLibrary(string directory);

        BookViewModel OrderForward(PageViewModel page, BookViewModel book);

        BookViewModel OrderBackward(PageViewModel page, BookViewModel book);

        Task SaveBookContentsOrder(BookViewModel book);

        void UpdateInMemory(BookViewModel book);

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
    }

    public delegate void SearchedEventHandler(object sender, SearchedEventArgs e);
}
