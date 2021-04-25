

using Homura.ORM;
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
                BookID = reader.SafeGetGuid("BookID", Table),
                Channel = reader.SafeGetInt("Channel", Table),
                ValueOrder = reader.SafeGetInt("ValueOrder", Table),
                Value = reader.SafeGetInt("Value", Table),
                MaxX = reader.SafeGetInt("MaxX", Table),
                MaxY = reader.SafeGetInt("MaxY", Table)
            };
        }
    }
}
