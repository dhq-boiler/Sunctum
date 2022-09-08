

using Homura.ORM.Mapping;
using Sunctum.Domain.Models.Conversion;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(Version_2))]
    public class Image : Entry
    {
        private string _RelativeMasterPath;
        private long? _ByteSize;
        private bool _IsEncrypted;

        [Column("MasterPath", "TEXT", 2), NotNull]
        [Since(typeof(VersionOrigin))]
        public string RelativeMasterPath
        {
            [DebuggerStepThrough]
            get
            { return _RelativeMasterPath; }
            set { SetProperty(ref _RelativeMasterPath, value); }
        }

        [Column("ByteSize", "INTEGER", 3, Homura.ORM.HandlingDefaultValue.AsValue)]
        [Since(typeof(Version_1))]
        public long? ByteSize
        {
            [DebuggerStepThrough]
            get
            { return _ByteSize; }
            set { SetProperty(ref _ByteSize, value); }
        }

        [Column("IsEncrypted", "INTEGER", 4, Homura.ORM.HandlingDefaultValue.AsValue, false)]
        [NotNull]
        [Since(typeof(Version_2))]
        public bool IsEncrypted
        {
            [DebuggerStepThrough]
            get { return _IsEncrypted; }
            set { SetProperty(ref _IsEncrypted, value); }
        }
    }
}
