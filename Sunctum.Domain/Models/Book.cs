

using Sunctum.Domain.Models.Conversion;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(Version_1))]
    public class Book : Entry
    {
        private Guid _AuthorID;
        private long? _ByteSize;

        [Column("AuthorID", "NUMERIC", 2)]
        [Since(typeof(VersionOrigin))]
        public Guid AuthorID
        {
            [DebuggerStepThrough]
            get
            { return _AuthorID; }
            set { SetProperty(ref _AuthorID, value); }
        }

        [Column("PublishDate", "NUMERIC", 3)]
        [Since(typeof(VersionOrigin))]
        public DateTime? PublishDate { get; internal set; }

        [Column("ByteSize", "INTEGER", 4)]
        [Since(typeof(Version_1))]
        public long? ByteSize
        {
            [DebuggerStepThrough]
            get
            { return _ByteSize; }
            set { SetProperty(ref _ByteSize, value); }
        }
    }
}
