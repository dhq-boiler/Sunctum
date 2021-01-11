

using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Sunctum.Domain.Models.Managers
{
    public interface IBookStorage : IObservable<BookCollectionChanged>
    {
        ObservableCollection<BookViewModel> BookSource { get; set; }
        ObservableCollection<BookViewModel> OnStage { get; }

        void AccessDispatcherObject(Action accessAction);
        void AddToMemory(BookViewModel book);
        void RemoveFromMemory(BookViewModel book);
        void UpdateInMemory(BookViewModel book);
        void FireFillContents(BookViewModel book);
        void RunFillContents(BookViewModel book);
        void FireFillContentsWithImage(BookViewModel book);
        void RunFillContentsWithImage(BookViewModel book);
    }
}