

using System;
using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public class BookTagViewModel : EntityBaseObjectViewModel
    {
        private Guid _BookID;
        private Guid _tagID;

        public BookTagViewModel()
        { }

        public BookTagViewModel(Guid bookId, Guid tagId)
        {
            _BookID = bookId;
            _tagID = tagId;
        }

        public BookTagViewModel(BookViewModel book, TagViewModel tag)
        : this(book.ID, tag.ID)
        { }

        public Guid BookID
        {
            [DebuggerStepThrough]
            get
            { return _BookID; }
            set { SetProperty(ref _BookID, value); }
        }

        public Guid TagID
        {
            [DebuggerStepThrough]
            get
            { return _tagID; }
            set { SetProperty(ref _tagID, value); }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null || obj.ToString().Equals("{DisconnectedItem}"))
                return false;
            var other = obj as BookTagViewModel;
            return BookID.Equals(other.BookID) && TagID.Equals(other.TagID);
        }

        public override int GetHashCode()
        {
            return BookID.GetHashCode() ^ TagID.GetHashCode();
        }
    }
}
