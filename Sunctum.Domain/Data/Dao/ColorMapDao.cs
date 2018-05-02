

using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    internal class ColorMapDao : SQLiteBaseDao<ColorMap>
    {
        public ColorMapDao() : base()
        { }

        public ColorMapDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override ColorMap ToEntity(IDataRecord reader)
        {
            return new ColorMap()
            {
                BookID = reader.SafeGetGuid("BookID"),
                Channel = reader.SafeGetInt("Channel"),
                ValueOrder = reader.SafeGetInt("ValueOrder"),
                Value = reader.SafeGetInt("Value"),
                MaxX = reader.SafeGetInt("MaxX"),
                MaxY = reader.SafeGetInt("MaxY")
            };
        }
    }
}
