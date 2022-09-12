

using Homura.Core;
using NLog;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace Sunctum.Managers
{
    public class BookStorage : BindableBase, IBookStorage
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ReactiveCollection<BookViewModel> _LoadedBooks;
        protected IObserver<BookCollectionChanged> observer;
        private FillContentsTaskManager _fcTaskManager = new FillContentsTaskManager();
        private bool disposedValue;

        public BookStorage()
        {
            _LoadedBooks = new ReactiveCollection<BookViewModel>();
        }

        public BookStorage(ReactiveCollection<BookViewModel> collection)
        {
            _LoadedBooks = new ReactiveCollection<BookViewModel>();
            BookSource.AddRange(collection);
        }

        public ReactiveCollection<BookViewModel> BookSource => _LoadedBooks;

        public virtual ReactiveCollection<BookViewModel> OnStage => BookSource;

        #region オンメモリ

        public void AddToMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_AddToMemory(book));
        }

        public void UpdateInMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_UpdateInMemory(book));
        }

        public void RemoveFromMemory(BookViewModel book)
        {
            AccessDispatcherObject(() => Internal_RemoveBookFromMemory(book));
        }

        #region private

        protected void Internal_AddToMemory(BookViewModel book)
        {
            BookSource.Add(book);
            if (observer is not null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Add()
                });
            }
            else
            {
                s_logger.Warn($"observer is null!");
            }
            RaisePropertyChanged(nameof(OnStage));
        }

        protected virtual void Internal_UpdateInMemory(BookViewModel book)
        {
            BookFacade.Update(book);
            int index = BookSource.IndexOf(BookSource.Where(b => b.ID.Equals(book.ID)).Single());
            BookSource[index] = book;
            if (observer is not null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Update()
                });
            }
            else
            {
                s_logger.Warn($"observer is null!");
            }
            RaisePropertyChanged(nameof(OnStage));
        }

        protected void Internal_RemoveBookFromMemory(BookViewModel book)
        {
            BookSource.Remove(book);
            if (observer is not null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Remove()
                });
            }
            else
            {
                s_logger.Warn($"observer is null!");
            }
            RaisePropertyChanged(nameof(OnStage));
        }

        #endregion //private

        #endregion //オンメモリ

        #region DispatcherObjectアクセス

        public void AccessDispatcherObject(Action accessAction)
        {
            if (Application.Current?.Dispatcher == null) //For UnitTest
            {
                accessAction.Invoke();
                return;
            }

            try
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    accessAction.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(accessAction);
                }
            }
            catch (NullReferenceException)
            { }
        }

        #endregion

        #region IObserver<BookCollectionChanged>

        public IDisposable Subscribe(IObserver<BookCollectionChanged> observer)
        {
            this.observer = observer;
            return new BookStorageDisposable(this);
        }

        public class BookStorageDisposable : IDisposable
        {
            private BookStorage _observable;

            public BookStorageDisposable(BookStorage observable)
            {
                _observable = observable;
            }

            public void Dispose()
            {
                _observable.observer = null;
            }
        }

        #endregion //IObserver<BookCollectionChanged>

        #region コンテンツ読み込み

        public void FireFillContents(BookViewModel book)
        {
            _fcTaskManager.RunAsync((b) => Internal_FillContents(b), book);
        }

        public void RunFillContents(BookViewModel book)
        {
            _fcTaskManager.Run((b) => Internal_FillContents(b), book);
        }

        public void FireFillContentsWithImage(BookViewModel book)
        {
            _fcTaskManager.RunAsync((b) => Internal_FillContentsWithImage(b), book);
        }

        public void RunFillContentsWithImage(BookViewModel book)
        {
            _fcTaskManager.Run((b) => Internal_FillContentsWithImage(b), book);
        }

        private void Internal_FillContents(BookViewModel book)
        {
            int currentCount = Querying.BookContentsCount(book.ID);
            InsertContentsObjIf(book);
            var countIsNotFull = currentCount != book.Contents.Count();
            var allPagesIsLoaded = book.Contents.All(b => b.IsLoaded);
            if (countIsNotFull || !allPagesIsLoaded)
            {
                ContentsLoadTask.FillContents(this, book);
            }
        }

        private void InsertContentsObjIf(BookViewModel book)
        {
            if (book.Contents == null)
            {
                book.Contents = new ObservableCollection<PageViewModel>();
            }
        }

        private void Internal_FillContentsWithImage(BookViewModel book)
        {
            int currentCount = Querying.BookContentsCount(book.ID);
            if (currentCount != book.Contents.Count() || !book.Contents.All(b => b.IsLoaded))
            {
                ContentsLoadTask.FillContentsWithImage(this, book);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    _LoadedBooks.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                _LoadedBooks = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion //コンテンツ読み込み
    }
}
