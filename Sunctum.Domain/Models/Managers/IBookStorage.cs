

using Reactive.Bindings;
using Sunctum.Domain.ViewModels;
using System;

namespace Sunctum.Domain.Models.Managers
{
    public interface IBookStorage : IObservable<BookCollectionChanged>, IDisposable
    {
        ReactiveCollection<BookViewModel> BookSource { get; }
        ReactiveCollection<BookViewModel> OnStage { get; }

        void AccessDispatcherObject(Action accessAction);
        void AddToMemory(BookViewModel book);
        void RemoveFromMemory(BookViewModel book);
        void UpdateInMemory(BookViewModel book);
        void FireFillContents(BookViewModel book);
        void RunFillContents(BookViewModel book);
        void FireFillContentsWithImage(BookViewModel book);
        void RunFillContentsWithImage(BookViewModel book);
        bool IsDirty(BookViewModel book);
    }
}