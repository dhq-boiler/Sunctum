

using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    internal class StarDao : SQLiteBaseDao<Star>
    {
        public StarDao()
            : base()
        { }

        public StarDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Star ToEntity(IDataRecord reader)
        {
            return new Star()
            {
                ID = reader.SafeGetGuid("ID"),
                TypeId = reader.SafeGetInt("TypeId"),
                Level = reader.SafeGetInt("Level")
            };
        }
    }
}
