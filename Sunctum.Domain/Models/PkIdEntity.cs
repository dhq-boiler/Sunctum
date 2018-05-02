

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    public abstract class PkIdEntity : EntityBaseObject, IId
    {
        private Guid _ID;

        [Column("ID", "NUMERIC", 0), PrimaryKey, Index]
        [Since(typeof(VersionOrigin))]
        public Guid ID
        {
            [DebuggerStepThrough]
            get
            { return _ID; }
            set { SetProperty(ref _ID, value); }
        }
    }
}
