

using Reactive.Bindings;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Sunctum.Managers
{
    public class BookCabinet : ArrangedBookStorage, IObserver<BookCollectionChanged>
    {
        public BookCabinet(ReactiveCollection<BookViewModel> collection)
            : base(collection)
        { }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(BookCollectionChanged value)
        {
            value.TargetChange.ApplyChange(BookSource, value.Target);
            RaisePropertyChanged(nameof(BookSource));
            RaisePropertyChanged(nameof(OnStage));
        }
    }
}
