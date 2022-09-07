using Homura.Extensions;
using Sunctum.Domain.Models;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class BookTagDao : SQLiteBaseDao<BookTag>
    {
        protected override BookTag ToEntity(IDataRecord reader)
        {
            return new BookTag()
            {
                BookID = reader.SafeGetGuid("BookID", Table),
                TagID = reader.SafeGetGuid("TagID", Table)
            };
        }
    }
}
