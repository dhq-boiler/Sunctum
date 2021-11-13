using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sunctum.Domain.Logic.Async.CurrentProcessProgress;

namespace Sunctum.Domain.Logic.Async
{
    public class CurrentProcessProgress : BindableBase, IObservable<CurrentProcessProgressObservable>
    {
        public ReactivePropertySlim<int> Count { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> TotalCount { get; } = new ReactivePropertySlim<int>();

        public ReadOnlyReactivePropertySlim<double> Rate { get; }

        public CurrentProcessProgress()
        {
            Rate = Count.CombineLatest(TotalCount, (a, b) => (double)a / b).ToReadOnlyReactivePropertySlim();
        }

        private List<IObserver<CurrentProcessProgressObservable>> _observers = new List<IObserver<CurrentProcessProgressObservable>>();

        public IDisposable Subscribe(IObserver<CurrentProcessProgressObservable> observer)
        {
            _observers.Add(observer);
            observer.OnNext(new CurrentProcessProgressObservable());
            return new CurrentProcessProgressDisposable(this, observer);
        }

        public class CurrentProcessProgressDisposable : IDisposable
        {
            private CurrentProcessProgress layer;
            private IObserver<CurrentProcessProgressObservable> observer;

            public CurrentProcessProgressDisposable(CurrentProcessProgress layer, IObserver<CurrentProcessProgressObservable> observer)
            {
                this.layer = layer;
                this.observer = observer;
            }

            public void Dispose()
            {
                layer._observers.Remove(observer);
            }
        }

        public class CurrentProcessProgressObservable : BindableBase
        {
        }
    }
}
