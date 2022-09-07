

using Homura.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class RecentOpenedLibraryDao : SQLiteBaseDao<RecentOpenedLibrary>
    {
        public RecentOpenedLibraryDao()
            : base()
        { }

        public RecentOpenedLibraryDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override RecentOpenedLibrary ToEntity(IDataRecord reader)
        {
            return new RecentOpenedLibrary()
            {
                Path = reader.SafeGetString("Path", Table),
                AccessOrder = reader.SafeGetInt("AccessOrder", Table)
            };
        }
    }
}
