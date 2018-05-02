

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;

namespace Sunctum.Domain.Models
{
    public class DirectoryNameParser : EntityBaseObject
    {
        private int _priority;
        private string _pattern;

        [Column("Priority", "INTEGER", 0), PrimaryKey, Index]
        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        [Column("Pattern", "Text", 1)]
        public string Pattern
        {
            get { return _pattern; }
            set { SetProperty(ref _pattern, value); }
        }
    }
}
