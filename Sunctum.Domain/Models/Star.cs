

using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(VersionOrigin))]
    public class Star : PkIdEntity
    {
        private int _TypeId;
        private int? _Level;

        [Column("TypeId", "INTEGER", 1)]
        [Since(typeof(VersionOrigin))]
        public int TypeId
        {
            [DebuggerStepThrough]
            get
            { return _TypeId; }
            set { SetProperty(ref _TypeId, value); }
        }

        [Column("Level", "INTEGER", 2)]
        [Since(typeof(VersionOrigin))]
        public int? Level
        {
            [DebuggerStepThrough]
            get
            { return _Level; }
            set { SetProperty(ref _Level, value); }
        }
    }
}
