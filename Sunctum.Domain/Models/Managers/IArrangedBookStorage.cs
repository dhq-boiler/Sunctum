﻿

using Reactive.Bindings;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.DisplayType;
using Sunctum.Domain.ViewModels;
using System;

namespace Sunctum.Domain.Models.Managers
{
    public interface IArrangedBookStorage : IBookStorage
    {
        bool IsSearching { get; }
        ReactiveCollection<BookViewModel> SearchedBooks { get; set; }
        IBookSorting Sorting { get; set; }
        string SearchText { get; set; }

        IDisplayType DisplayType { get; set; }

        event EventHandler SearchCleared;
        event SearchedEventHandler Searched;

        void ClearSearchResult();
        void Search();
        void Search(string searchingText);
        bool SortingSelected(string name);
    }
}