﻿

using Ninject;
using NLog;
using Prism.Mvvm;
using Sunctum.Core.Notifications;
using Sunctum.Domail.Util;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.AuthorSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sunctum.Managers
{
    public class AuthorManager : BindableBase, IAuthorManager, IObserver<ActiveTabChanged>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<AuthorViewModel> _Authors;
        private List<AuthorViewModel> _SelectedItems;
        private bool _EnableOrderByName;
        private bool _OrderAscending;
        private ObservableCollection<AuthorCountViewModel> _AuthorCount;
        private ObservableCollection<AuthorCountViewModel> _SearchedAuthors;
        private IAuthorSorting _AuthorSorting;

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        public IProgressManager ProgressManager { get; set; } = new ProgressManager();

        public AuthorManager()
        {
            RegisterCommands();
            _AuthorCount = new ObservableCollection<AuthorCountViewModel>();
            _AuthorSorting = AuthorSorting.ByNameAsc;
        }

        public void Load()
        {
            SelectedItems = new List<AuthorViewModel>();
            Authors = new ObservableCollection<AuthorViewModel>(AuthorFacade.FindAll());
            Sorting = AuthorSorting.ByNameAsc;
            LoadAuthorCount();
        }

        public async Task LoadAsync()
        {
            await Task.Run(() => Load());
        }

        private void Filter(ObservableCollection<BookViewModel> bookSource)
        {
            var bookAuthorSet = new HashSet<Guid>(bookSource.Select(b => b.AuthorID));
            var timeKeeper = new TimeKeeper();
            ProgressManager.UpdateProgress(0, AuthorCount.Count, timeKeeper);

            var i = 0;
            foreach (var authorCount in AuthorCount)
            {
                authorCount.IsVisible = bookAuthorSet.Contains(authorCount.Author.ID);
                ++i;
                ProgressManager.UpdateProgress(i, AuthorCount.Count, timeKeeper);
            }
            ProgressManager.Complete();
        }

        public void LoadedBooks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AuthorCountInsertOrIncrement(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    AuthorCountDecrement(e);
                    AuthorCountInsertOrIncrement(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    AuthorCountDecrement(e);
                    break;
            }
        }

        private void AuthorCountDecrement(NotifyCollectionChangedEventArgs e)
        {
            foreach (var removeBook in e.OldItems.Cast<BookViewModel>().Where(b => b.Author != null))
            {
                if (AuthorCount.Where(ac => ac.Author.Name == removeBook.Author.Name).Count() == 1)
                {
                    var authorCount = AuthorCount.Single(ac => ac.Author.Name == removeBook.Author.Name);
                    authorCount.Count--;
                }
            }
        }

        private void AuthorCountInsertOrIncrement(NotifyCollectionChangedEventArgs e)
        {
            foreach (var newBook in e.NewItems.Cast<BookViewModel>().Where(b => b.Author != null))
            {
                if (AuthorCount.Where(ac => ac.Author.Name == newBook.Author.Name).Count() == 1)
                {
                    var authorCount = AuthorCount.Single(ac => ac.Author.Name == newBook.Author.Name);
                    authorCount.Count++;
                }
                else if (AuthorCount.Where(ac => ac.Author.Name == newBook.Author.Name).Count() == 0)
                {
                    AuthorCount.Add(new AuthorCountViewModel(newBook.Author, 1));
                }
            }
        }

        public void LoadAuthorCount()
        {
            AuthorCount = new ObservableCollection<AuthorCountViewModel>(GenerateAuthorCount());
        }

        private void RegisterCommands()
        {
        }

        public ObservableCollection<AuthorViewModel> Authors
        {
            [DebuggerStepThrough]
            get
            { return _Authors; }
            set
            {
                SetProperty(ref _Authors, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => AuthorCount));
            }
        }

        public List<AuthorViewModel> SelectedItems
        {
            [DebuggerStepThrough]
            get
            { return _SelectedItems; }
            set { SetProperty(ref _SelectedItems, value); }
        }

        public ObservableCollection<AuthorCountViewModel> AuthorCount
        {
            [DebuggerStepThrough]
            get
            { return _AuthorCount; }
            set
            {
                SetProperty(ref _AuthorCount, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public ObservableCollection<AuthorCountViewModel> SearchedAuthorCounts
        {
            [DebuggerStepThrough]
            get
            { return _SearchedAuthors; }
            set
            {
                SetProperty(ref _SearchedAuthors, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => IsSearching));
            }
        }

        private ObservableCollection<AuthorCountViewModel> DisplayableAuthorCountSource
        {
            [DebuggerStepThrough]
            get
            {
                if (SearchedAuthorCounts != null)
                {
                    return SearchedAuthorCounts;
                }
                else
                {
                    return AuthorCount;
                }
            }
        }

        public ObservableCollection<AuthorCountViewModel> OnStage
        {
            get
            {
                if (EnableOrderByName)
                {
                    if (_OrderAscending)
                    {
                        Sorting = AuthorSorting.ByNameAsc;
                    }
                    else
                    {
                        Sorting = AuthorSorting.ByNameDesc;
                    }
                }
                else
                {
                    if (_OrderAscending)
                    {
                        Sorting = AuthorSorting.ByCountAsc;
                    }
                    else
                    {
                        Sorting = AuthorSorting.ByCountDesc;
                    }
                }

                var newCollection = Sorting.Sort(DisplayableAuthorCountSource).ToArray();
                return new ObservableCollection<AuthorCountViewModel>(newCollection);
            }
        }

        public bool IsSearching
        {
            [DebuggerStepThrough]
            get
            { return SearchedAuthorCounts != null; }
        }

        public bool EnableOrderByName
        {
            get { return _EnableOrderByName; }
            set
            {
                SetProperty(ref _EnableOrderByName, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public string OrderText
        {
            get
            {
                if (_OrderAscending)
                    return "↑";
                else
                    return "↓";
            }
        }

        public IAuthorSorting Sorting
        {
            [DebuggerStepThrough]
            get
            { return _AuthorSorting; }
            set
            {
                SetProperty(ref _AuthorSorting, value);
            }
        }

        public void SwitchOrdering()
        {
            _OrderAscending = !_OrderAscending;

            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OrderText));
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        private IEnumerable<AuthorCountViewModel> GenerateAuthorCount()
        {
            if (Authors == null)
            {
                return new List<AuthorCountViewModel>();
            }

            Stopwatch sw = new Stopwatch();
            s_logger.Info($"Loading AuthorCount list...");
            sw.Start();

            try
            {
                var authors = Authors.ToList();

                if (EnableOrderByName)
                {
                    if (_OrderAscending)
                    {
                        return AuthorFacade.FindAllAsCountOrderByNameAsc();
                    }
                    else
                    {
                        return AuthorFacade.FindAllAsCountOrderByNameDesc();
                    }
                }
                else
                {
                    if (_OrderAscending)
                    {
                        return AuthorFacade.FindAllAsCountOrderByCountAsc();
                    }
                    else
                    {
                        return AuthorFacade.FindAllAsCountOrderByCountDesc();
                    }
                }
            }
            finally
            {
                sw.Stop();
                s_logger.Info($"Completed to load AuthorCount list. {sw.ElapsedMilliseconds}ms");
            }
        }

        public void ObserveAuthorCount()
        {
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => AuthorCount));
        }

        public void ShowBySelectedItems()
        {
            var activeViewModel = MainWindowViewModel.ActiveDocumentViewModel;

            var books = from b in activeViewModel.BookCabinet.BookSource
                        join s in SelectedItems on b.AuthorID equals s.ID
                        select b;

            activeViewModel.BookCabinet.SearchedBooks = new ObservableCollection<BookViewModel>(books.ToList());
        }

        public void ShowBySelectedItems(IEnumerable<AuthorViewModel> searchItems)
        {
            SelectedItems = searchItems.ToList();

            ShowBySelectedItems();
        }

        public void ClearSearchResult()
        {
            if (_AuthorCount == null) return;

            foreach (var item in _AuthorCount)
            {
                item.IsSearchingKey = false;
            }
        }

        public bool AnySearchingKeys()
        {
            if (_AuthorCount == null) return false;

            return _AuthorCount.Any(a => a.IsSearchingKey);
        }

        private object _lock_object = new object();
        private CancellationTokenSource _tokenSource;

        public void OnNext(ActiveTabChanged value)
        {
            lock (_lock_object)
            {
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                }
            }
            _tokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                Filter(value.BookStorage.BookSource);
                lock (_lock_object)
                {
                    _tokenSource = null;
                }
            }, _tokenSource.Token);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
