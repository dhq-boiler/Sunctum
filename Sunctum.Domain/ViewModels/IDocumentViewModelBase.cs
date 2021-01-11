

using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sunctum.Domain.ViewModels
{
    public interface IDocumentViewModelBase
    {
        IArrangedBookStorage BookCabinet { get; set; }
        ObservableCollection<BookViewModel> BookListViewSelectedItems { get; set; }
        ObservableCollection<Control> BooksContextMenuItems { get; set; }
        bool CanClose { get; }
        ObservableCollection<Control> ContentsContextMenuItems { get; set; }
        List<PageViewModel> ContentsListViewSelectedItems { get; set; }
        int CurrentPage { get; }
        bool IsVisible { get; set; }
        ILibrary LibraryManager { get; set; }
        IMainWindowViewModel MainWindowViewModel { get; set; }
        BookViewModel OpenedBook { get; set; }
        PageViewModel OpenedPage { get; set; }
        bool SearchPaneIsVisible { get; set; }
        string SearchStatusText { get; }
        string SearchText { get; set; }
        List<EntryViewModel> SelectedEntries { get; }
        string Title { get; }
        string ContentId { get; }
        string UnescapedSearchText { get; set; }

        event EventHandler SearchCleared;
        event SearchedEventHandler Searched;

        void AddToSelectedEntries(IEnumerable<EntryViewModel> add);
        void AddToSelectedEntry(EntryViewModel add);
        void BuildContextMenus_Books();
        void BuildContextMenus_Contents();
        void ClearSearchResult();
        void ClearSelectedItems();
        void CloseBook();
        void CloseImage();
        void CloseSearchPane();
        void GoNextImage();
        void GoPreviousImage();
        Task ImportAsync(string[] objectPaths);
        void MovePageBackward(PageViewModel page);
        void MovePageForward(PageViewModel page);
        void OpenBook(BookViewModel book);
        void OpenImage(PageViewModel page);
        void OpenSearchPane();
        Task RemoveBook(BookViewModel[] books);
        void RemoveFromSelectedEntries(IEnumerable<EntryViewModel> entries);
        Task RemovePage(IEnumerable<PageViewModel> pages);
        void ResetScrollOffset();
        void ResetScrollOffsetPool();
        void RestoreScrollOffset(Guid bookId);
        Task SaveOpenedBookContentsOrder();
        void Search();
        void Search(string searchingText);
        bool SortingSelected(string name);
        void StoreScrollOffset(Guid bookId);
    }
}