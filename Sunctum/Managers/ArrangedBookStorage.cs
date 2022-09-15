

using Homura.Core;
using NLog;
using Reactive.Bindings;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.DisplayType;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sunctum.Managers
{
    public class ArrangedBookStorage : BookStorage, IArrangedBookStorage
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ReactiveCollection<BookViewModel> _SearchedBooks;
        private IBookSorting _BookSorting;
        private IDisplayType _DisplayType;
        private string _SearchText;

        public ArrangedBookStorage()
            : base()
        {
            Sorting = BookSorting.ByLoadedAsc;
            DisplayType = Sunctum.Domain.Logic.DisplayType.DisplayType.SideBySide;
        }

        public ArrangedBookStorage(ReactiveCollection<BookViewModel> collection)
            : base(collection)
        {
            Sorting = BookSorting.ByLoadedAsc;
            DisplayType = Sunctum.Domain.Logic.DisplayType.DisplayType.SideBySide;
        }

        public ReactiveCollection<BookViewModel> SearchedBooks
        {
            [DebuggerStepThrough]
            get
            { return _SearchedBooks; }
            set
            {
                SetProperty(ref _SearchedBooks, value);
                RaisePropertyChanged(nameof(OnStage));
                RaisePropertyChanged(nameof(IsSearching));
            }
        }

        private ReactiveCollection<BookViewModel> DisplayableBookSource
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

        public override ReactiveCollection<BookViewModel> OnStage
        {
            get
            {
                var ret = new ReactiveCollection<BookViewModel>();
                ret.AddRange(Sorting.Sort(DisplayableBookSource));
                return ret;
            }
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

        public IDisplayType DisplayType
        {
            [DebuggerStepThrough]
            get
            { return _DisplayType; }
            set
            {
                SetProperty(ref _DisplayType, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public string SearchText
        {
            [DebuggerStepThrough]
            get
            { return _SearchText; }
            set
            {
                SetProperty(ref _SearchText, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => SearchStatusText));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => UnescapedSearchText));
            }
        }

        public string UnescapedSearchText
        {
            [DebuggerStepThrough]
            get
            { return HttpUtility.HtmlDecode(SearchText); }
            set { SearchText = HttpUtility.HtmlEncode(value); }
        }

        public string SearchStatusText
        {
            get { return $"Searched by '{UnescapedSearchText}'"; }
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

        public void Search()
        {
            Search(SearchText);
        }

        public void Search(string searchingText)
        {
            _SearchText = searchingText;
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
                    SearchedBooks = new ReactiveCollection<BookViewModel>();
                    SearchedBooks.AddRange(BookSource.Where(b => AuthorNameContainsSearchText(b, searchingText) || TitleContainsSearchText(b, searchingText) || FingerPrintContainsSearchText(b, searchingText)));
                    RaisePropertyChanged(nameof(OnStage));
                    OnSearched(new SearchedEventArgs(searchingText, _previousSearchingText));
                }

                _previousSearchingText = searchingText;
            });
        }

        protected bool AuthorNameContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null || target.Author is null)
            {
                return false;
            }
            return target.Author.Name.IndexOf(searchingText) != -1;
        }

        protected bool TitleContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null || target.Title is null)
            {
                return false;
            }
            return target.Title.IndexOf(searchingText) != -1;
        }

        protected bool FingerPrintContainsSearchText(BookViewModel target, string searchingText)
        {
            if (target == null || target.FingerPrint is null)
            {
                return false;
            }
            return target.FingerPrint.IndexOf(searchingText) != -1;
        }

        public void ClearSearchResult()
        {
            SearchText = "";
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
