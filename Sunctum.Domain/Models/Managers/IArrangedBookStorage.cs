

using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Sunctum.Domain.Models.Managers
{
    public interface IArrangedBookStorage : IBookStorage
    {
        bool IsSearching { get; }
        ObservableCollection<BookViewModel> SearchedBooks { get; set; }
        IBookSorting Sorting { get; set; }

        event EventHandler SearchCleared;
        event SearchedEventHandler Searched;

        void ClearSearchResult();
        void Search(string searchingText);
        bool SortingSelected(string name);
    }
}