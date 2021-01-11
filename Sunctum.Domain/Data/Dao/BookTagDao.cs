

using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class BookTagDao : SQLiteBaseDao<BookTag>
    {
        protected override BookTag ToEntity(IDataRecord reader)
        {
            return new BookTag()
            {
                BookID = reader.SafeGetGuid("BookID"),
                TagID = reader.SafeGetGuid("TagID")
            };
        }
    }
}
