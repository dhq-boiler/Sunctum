

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using YamlDotNet.Serialization;

namespace Sunctum.Domain.ViewModels
{
    public class BookViewModel : EntryViewModel, ICloneable, IDisposable
    {
        private Configuration _Configuration;
        private PageViewModel _FirstPage;
        private bool _ContentsRegistered;
        private ObservableCollection<PageViewModel> _Contents;
        private Guid _AuthorID;
        private AuthorViewModel _Author;
        private long? _ByteSize;
        private string _FingerPrint;

        public BookViewModel()
        {
            Contents = new ObservableCollection<PageViewModel>();
            NumberOfPages = Contents
                .PropertyChangedAsObservable()
                .Select(x => Contents.Count())
                .ToReactiveProperty();
        }

        public BookViewModel(Guid id, string title)
            : base(id, title)
        {
            Contents = new ObservableCollection<PageViewModel>();
            NumberOfPages = Contents
                .PropertyChangedAsObservable()
                .Select(x => Contents.Count())
                .ToReactiveProperty();
        }

        public Configuration Configuration
        {
            [DebuggerStepThrough]
            get
            { return _Configuration; }
            set { SetProperty(ref _Configuration, value); }
        }

        public ObservableCollection<PageViewModel> Contents
        {
            [DebuggerStepThrough]
            get
            { return _Contents; }
            set { SetProperty(ref _Contents, value); }
        }

        [YamlIgnore]
        public ReactiveProperty<int> NumberOfPages { get; set; }

        public Guid AuthorID
        {
            [DebuggerStepThrough]
            get
            { return _AuthorID; }
            set { SetProperty(ref _AuthorID, value); }
        }

        public DateTime? PublishDate { get; set; }

        public long? ByteSize
        {
            [DebuggerStepThrough]
            get
            { return _ByteSize; }
            set { SetProperty(ref _ByteSize, value); }
        }

        public string FingerPrint
        {
            [DebuggerStepThrough]
            get
            { return _FingerPrint; }
            set { SetProperty(ref _FingerPrint, value); }
        }

        public PageViewModel FirstPage
        {
            [DebuggerStepThrough]
            get
            { return _FirstPage; }
            set { SetProperty(ref _FirstPage, value); }
        }

        public AuthorViewModel Author
        {
            [DebuggerStepThrough]
            get
            { return _Author; }
            set
            {
                SetProperty(ref _Author, value);
                if (value == null)
                {
                    AuthorID = Guid.Empty;
                }
                else
                {
                    AuthorID = value.ID;
                }
            }
        }

        public bool ContentsRegistered
        {
            [DebuggerStepThrough]
            get
            { return _ContentsRegistered; }
            set { SetProperty(ref _ContentsRegistered, value); }
        }

        public PageViewModel this[int index]
        {
            get { return Contents[index]; }
            set
            {
                if (index >= 0 && index < Contents.Count)
                {
                    Contents[index] = value;
                }
                else if (Contents.Count - 1 < index)
                {
                    ObservableCollection<PageViewModel> newContents = new ObservableCollection<PageViewModel>(new List<PageViewModel>(index + 1));
                    for (int i = 0; i <= index; ++i)
                    {
                        newContents[i] = Contents[i];
                    }
                    newContents[index] = value;
                    Contents = newContents;
                }
                else if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public void AddPage(PageViewModel page)
        {
            Contents.Add(page);
        }

        public void RemovePage(PageViewModel page)
        {
            Contents.Remove(page);
            if (FirstPage.ID == page.ID)
            {
                if (Contents.Count > 0)
                {
                    FirstPage = Contents.First();
                }
                else
                {
                    FirstPage = null;
                }
            }
        }

        public void ResetContents(IEnumerable<PageViewModel> pages)
        {
            Contents = new ObservableCollection<PageViewModel>(pages);
        }

        public void ClearContents()
        {
            Contents.Clear();
        }

        public void SetPage(int index, PageViewModel page)
        {
            this[index] = page;
        }

        public void ObservePage(int index)
        {
            OnPropertyChanged("Contents[" + index + "]");
        }

        public override string ToString()
        {
            string contentsStr = "";
            if (Contents == null)
            {
                return "{BookViewModel}";
            }
            foreach (var page in Contents)
            {
                contentsStr += page.ToString();
                if (!Contents.Last().Equals(page))
                    contentsStr += ", ";
            }
            return "{NumberOfPages=" + NumberOfPages + ", PublishDate=" + PublishDate + ", Contents=[" + contentsStr + "]}";
        }

        public object Clone()
        {
            return new BookViewModel()
            {
                Configuration = Configuration.ApplicationConfiguration,
                ID = this.ID,
                Author = this.Author,
                AuthorID = this.AuthorID,
                FirstPage = this.FirstPage,
                PublishDate = this.PublishDate,
                ByteSize = this.ByteSize,
                Title = this.Title,
                Contents = this.Contents,
                ContentsRegistered = this.ContentsRegistered,
                IsLoaded = this.IsLoaded
            };
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BookViewModel)) return false;
            BookViewModel book = obj as BookViewModel;

            return ID.Equals(book.ID)
                && Object.Equals(AuthorID, book.AuthorID)
                && Object.Equals(PublishDate, book.PublishDate)
                && Object.Equals(ByteSize, book.ByteSize)
                && Title.Equals(book.Title);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode()
                ^ AuthorID.GetHashCode()
                ^ PublishDate.GetHashCode()
                ^ ByteSize.GetHashCode()
                ^ Title.GetHashCode();
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NumberOfPages.Dispose();
                }

                Contents = null;
                FirstPage = null;
                NumberOfPages = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
