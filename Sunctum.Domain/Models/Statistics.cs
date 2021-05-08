

using Homura.ORM;
using Homura.ORM.Mapping;

namespace Sunctum.Domain.Models
{
    public class Statistics : PkIdEntity
    {
        private int _number_of_boots;

        [Column("NumberOfBoots", "INTEGER", 1)]
        public int NumberOfBoots
        {
            get { return _number_of_boots; }
            set { SetProperty(ref _number_of_boots, value); }
        }
    }
}
