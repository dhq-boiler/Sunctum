﻿

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using System;
using System.Data;

namespace Sunctum.Infrastructure.Test.TestFixture.Dao
{
    public class HeaderDao : Dao<Header>
    {
        public HeaderDao()
            : base()
        { }

        public HeaderDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Header ToEntity(IDataRecord reader)
        {
            return new Header()
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
