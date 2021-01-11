

using Homura.ORM;
using Homura.ORM.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [PrimaryKey("BookID", "TagID")]
    [DefaultVersion(typeof(VersionOrigin))]
    public class BookTag : EntityBaseObject
    {
        private Guid _bookId;
        private Guid _tagId;

        [Column("BookID", "NUMERIC", 0)]
        public Guid BookID
        {
            [DebuggerStepThrough]
            get
            { return _bookId; }
            set { SetProperty(ref _bookId, value); }
        }

        [Column("TagID", "NUMERIC", 1)]
        public Guid TagID
        {
            [DebuggerStepThrough]
            get
            { return _tagId; }
            set { SetProperty(ref _tagId, value); }
        }
    }
}
