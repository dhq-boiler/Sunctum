

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using System;
using System.Data;

namespace Sunctum.Infrastructure.Test.TestFixture.Dao
{
    public class OriginDao : Dao<Origin>
    {
        public OriginDao()
            : base()
        { }

        public OriginDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Origin ToEntity(IDataRecord reader)
        {
            return new Origin()
            {
                Id = SafeGetGuid(reader, "Id"),
                Item1 = SafeGetString(reader, "Item1"),
                Item2 = SafeGetString(reader, "Item2"),
                Item3 = SafeGetString(reader, "Item3")
            };
        }

        private static Guid SafeGetGuid(IDataRecord rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            bool isNull = rdr.IsDBNull(index);

            return isNull ? Guid.Empty : rdr.GetGuid(index);
        }

        private static string SafeGetString(IDataRecord rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            bool isNull = rdr.IsDBNull(index);

            return isNull ? null : rdr.GetString(index);
        }
    }
}
