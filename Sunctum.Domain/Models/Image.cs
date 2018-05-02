

using Sunctum.Domain.Models.Conversion;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(Version_1))]
    public class Image : Entry
    {
        private string _RelativeMasterPath;
        private long? _ByteSize;

        [Column("MasterPath", "TEXT", 2), NotNull]
        [Since(typeof(VersionOrigin))]
        public string RelativeMasterPath
        {
            [DebuggerStepThrough]
            get
            { return _RelativeMasterPath; }
            set { SetProperty(ref _RelativeMasterPath, value); }
        }

        [Column("ByteSize", "INTEGER", 3)]
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
