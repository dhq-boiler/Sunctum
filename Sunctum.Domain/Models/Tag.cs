

using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(VersionOrigin))]
    public class Tag : PkIdEntity
    {
        private string _Name;

        [Column("Name", "TEXT", 1), NotNull]
        [Since(typeof(VersionOrigin))]
        public string Name
        {
            [DebuggerStepThrough]
            get
            { return _Name; }
            set
            {
                SetProperty(ref _Name, value);
            }
        }
    }
}
