

using Reactive.Bindings;
using Sunctum.Domain.ViewModels;
using System;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public interface IBookStorage : IObservable<BookCollectionChanged>, IDisposable
    {
        ReactiveCollection<BookViewModel> BookSource { get; }
        ReactiveCollection<BookViewModel> OnStage { get; }

        Task AccessDispatcherObject(Func<Task> accessAction);
        Task AddToMemory(BookViewModel book);
        Task RemoveFromMemory(BookViewModel book);
        Task UpdateInMemory(BookViewModel book);
        void FireFillContents(BookViewModel book);
        void RunFillContents(BookViewModel book);
        void FireFillContentsWithImage(BookViewModel book);
        void RunFillContentsWithImage(BookViewModel book);
        bool IsDirty(BookViewModel book);
    }
}