

using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public class AuthorCountViewModel : BaseObjectViewModel
    {
        private AuthorViewModel _Author;
        private int _Count;
        private bool _IsSearchingKey;
        private bool _IsVisible;

        public AuthorViewModel Author
        {
            [DebuggerStepThrough]
            get
            { return _Author; }
            set { SetProperty(ref _Author, value); }
        }

        public int Count
        {
            [DebuggerStepThrough]
            get
            { return _Count; }
            set { SetProperty(ref _Count, value); }
        }

        public bool IsSearchingKey
        {
            [DebuggerStepThrough]
            get
            { return _IsSearchingKey; }
            set { SetProperty(ref _IsSearchingKey, value); }
        }

        public bool IsVisible
        {
            [DebuggerStepThrough]
            get
            { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        public AuthorCountViewModel()
        { }

        public AuthorCountViewModel(AuthorViewModel author, int count)
        {
            Author = author;
            Count = count;
        }
    }
}
