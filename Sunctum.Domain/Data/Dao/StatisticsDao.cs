

using Homura.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class StatisticsDao : SQLiteBaseDao<Statistics>
    {
        public StatisticsDao()
            : base()
        { }

        public StatisticsDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Statistics ToEntity(IDataRecord reader)
        {
            return new Statistics()
            {
                ID = reader.SafeGetGuid("ID", Table),
                NumberOfBoots = reader.SafeGetInt("NumberOfBoots", Table),
            };
        }
    }
}
