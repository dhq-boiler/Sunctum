

using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Managers
{
    public class ArrangedBookStorage : BookStorage, IArrangedBookStorage
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<BookViewModel> _SearchedBooks;
        private IBookSorting _BookSorting;

        public ArrangedBookStorage()
            : base()
        {
            Sorting = BookSorting.ByLoadedAsc;
        }

        public ArrangedBookStorage(ObservableCollection<BookViewModel> collection)
            : base(collection)
        {
            Sorting = BookSorting.ByLoadedAsc;
        }

        public ObservableCollection<BookViewModel> SearchedBooks
        {
            [DebuggerStepThrough]
            get
            { return _SearchedBooks; }
            set
            {
                SetProperty(ref _SearchedBooks, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => IsSearching));
            }
        }

        private ObservableCollection<BookViewModel> DisplayableBookSource
        {
            [DebuggerStepThrough]
            get
            {
                if (SearchedBooks != null)
                {
                    return SearchedBooks;
                }
                else
                {
                    return BookSource;
                }
            }
        }

        public override ObservableCollection<BookViewModel> OnStage
        {
            get { return new ObservableCollection<BookViewModel>(Sorting.Sort(DisplayableBookSource)); }
        }

        public bool IsSearching
        {
            [DebuggerStepThrough]
            get
            { return SearchedBooks != null; }
        }

        public IBookSorting Sorting
        {
            [DebuggerStepThrough]
            get
            { return _BookSorting; }
            set
            {
                SetProperty(ref _BookSorting, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        protected override void Internal_UpdateInMemory(BookViewModel book)
        {
            BookFacade.Update(book);
            int index = BookSource.IndexOf(BookSource.Where(b => b.ID.Equals(book.ID)).Single());
            BookSource[index] = book;
            if (SearchedBooks != null)
            {
                index = SearchedBooks.IndexOf(SearchedBooks.Where(b => b.ID.Equals(book.ID)).Single());
                SearchedBooks[index] = book;
            }
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        #region 検索

        private string _previousSearchingText;

        public event EventHandler SearchCleared;
        public event SearchedEventHandler Searched;

        protected virtual void OnSearchCleared(EventArgs e)
        {
            SearchCleared?.Invoke(this, e);
        }

        protected virtual void OnSearched(SearchedEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        public void Search(string searchingText)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(searchingText))
                {
                    SearchedBooks = null;

                    OnSearchCleared(new EventArgs());
                }
                else
                {
                    s_logger.Debug($"Search Word:{searchingText}");
                    SearchedBooks = new ObservableCollection<BookViewModel>(BookSource.Where(b => AuthorNameContainsSearchText(b, searchingText) || TitleContainsSearchText(b, searchingText)));

                    OnSearched(new SearchedEventArgs(searchingText, _previousSearchingText));
                }

                _previousSearchingText = searchingText;
            });
        }

        private bool AuthorNameContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null || target.Author == null)
            {
                return false;
            }
            return target.Author.Name.IndexOf(searchingText) != -1;
        }

        private bool TitleContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null)
            {
                return false;
            }
            return target.Title.IndexOf(searchingText) != -1;
        }

        public void ClearSearchResult()
        {
            SearchedBooks = null;
        }

        #endregion //検索

        #region 問い合わせ

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(this.Sorting, name);
        }

        #endregion //問い合わせ
    }
}
