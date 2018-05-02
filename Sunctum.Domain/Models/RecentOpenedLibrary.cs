

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;

namespace Sunctum.Domain.Models
{
    public class RecentOpenedLibrary : EntityBaseObject
    {
        private string _path;
        private int _order;

        [Column("Path", "Text", 0), PrimaryKey, Index]
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        [Column("AccessOrder", "INTEGER", 1)]
        public int AccessOrder
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
    }
}
