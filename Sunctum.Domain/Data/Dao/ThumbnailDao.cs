

using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    internal class ThumbnailDao : SQLiteBaseDao<Thumbnail>
    {
        public ThumbnailDao() : base()
        { }

        public ThumbnailDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override Thumbnail ToEntity(IDataRecord reader)
        {
            return new Thumbnail()
            {
                ID = reader.SafeGetGuid("ID"),
                ImageID = reader.SafeGetGuid("ImageID"),
                RelativeMasterPath = reader.SafeGetString("Path"),
            };
        }
    }
}
