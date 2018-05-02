

using Sunctum.Infrastructure.Core;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    public class AuthorCount : BaseObject
    {
        private Author _Author;
        private int _Count;
        private bool _IsSearchingKey;

        public Author Author
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

        public AuthorCount()
        { }

        public AuthorCount(Author author, int count)
        {
            Author = author;
            Count = count;
        }
    }
}
