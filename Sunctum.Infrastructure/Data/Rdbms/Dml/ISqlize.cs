
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface ISqlize
    {
        string ToSql();

        string ToSql(Dictionary<string, int> placeholderNameDictionary);
    }
}
