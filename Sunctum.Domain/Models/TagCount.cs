

using Sunctum.Infrastructure.Core;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    public class TagCount : BaseObject
    {
        private Tag _Tag;
        private int _Count;
        private bool _IsSearchingKey;

        public Tag Tag
        {
            [DebuggerStepThrough]
            get
            { return _Tag; }
            set { SetProperty(ref _Tag, value); }
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

        public TagCount()
        { }

        public TagCount(Tag tag, int count)
        {
            Tag = tag;
            Count = count;
        }
    }
}
