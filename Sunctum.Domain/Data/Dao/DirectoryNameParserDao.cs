

using Homura.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class DirectoryNameParserDao : SQLiteBaseDao<DirectoryNameParser>
    {
        public DirectoryNameParserDao()
            : base()
        { }

        public DirectoryNameParserDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override DirectoryNameParser ToEntity(IDataRecord reader)
        {
            return new DirectoryNameParser()
            {
                Priority = reader.SafeGetInt("Priority", Table),
                Pattern = reader.SafeGetString("Pattern", Table)
            };
        }
    }
}
