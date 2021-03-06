﻿

using Homura.Core;
using NLog;
using Prism.Mvvm;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Sunctum.Managers
{
    public class BookStorage : BindableBase, IBookStorage
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<BookViewModel> _LoadedBooks;
        protected IObserver<BookCollectionChanged> observer;
        private FillContentsTaskManager _fcTaskManager = new FillContentsTaskManager();

        public BookStorage()
        {
            BookSource = new ObservableCollection<BookViewModel>();
        }

        public BookStorage(ObservableCollection<BookViewModel> collection)
        {
            BookSource = new ObservableCollection<BookViewModel>(collection); //copy elements, not copy reference
        }

        public ObservableCollection<BookViewModel> BookSource
        {
            [DebuggerStepThrough]
            get
            { return _LoadedBooks; }
            set
            {
                SetProperty(ref _LoadedBooks, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public virtual ObservableCollection<BookViewModel> OnStage
        {
            get
            {
                return BookSource;
            }
        }

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
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            if (observer != null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Add()
                });
            }
        }

        protected virtual void Internal_UpdateInMemory(BookViewModel book)
        {
            BookFacade.Update(book);
            int index = BookSource.IndexOf(BookSource.Where(b => b.ID.Equals(book.ID)).Single());
            BookSource[index] = book;
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            if (observer != null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Update()
                });
            }
        }

        protected void Internal_RemoveBookFromMemory(BookViewModel book)
        {
            BookSource.Remove(book);
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            if (observer != null)
            {
                observer.OnNext(new BookCollectionChanged()
                {
                    Target = book,
                    TargetChange = new BookCollectionChanged.Remove()
                });
            }
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
            if (currentCount != book.Contents.Count() || !book.Contents.All(b => b.IsLoaded))
            {
                ContentsLoadTask.FillContents(this, book);
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

        #endregion //コンテンツ読み込み
    }
}
