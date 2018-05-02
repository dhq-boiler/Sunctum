

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Logic.Parse;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sunctum.ViewModels
{
    internal class DirectoryNameParserViewModel : IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public DirectoryNameParser Model { get; }

        public ReactiveProperty<int> Priority { get; }

        public ReactiveProperty<string> Pattern { get; }

        private Subject<Unit> CommitTrigger { get; } = new Subject<Unit>();

        private IObservable<Unit> CommitAsObservable => this.CommitTrigger
            .ToUnit();

        public DirectoryNameParserViewModel(IDirectoryNameParserManager DirectoryNameParserManager, DirectoryNameParser model)
        {
            Model = model;
            Priority = Model
                .ObserveProperty(x => x.Priority)
                .ToReactiveProperty()
                .AddTo(Disposable);
            Pattern = Model
                .ObserveProperty(x => x.Pattern)
                .ToReactiveProperty()
                .AddTo(Disposable);
            CommitAsObservable
                .Select(_ => Pattern.Value)
                .Subscribe(x =>
                {
                    Model.Pattern = x;
                    if (DirectoryNameParserManager.Items.Where(i => i.Pattern.Equals(Model.Pattern)).Count() == 0)
                    {
                        Model.Priority = DirectoryNameParserManager.Items.Count;
                        DirectoryNameParserManager.Items.Add(Model);
                    }
                })
                .AddTo(Disposable);
        }

        public void Commit() => this.CommitTrigger.OnNext(Unit.Default);

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
