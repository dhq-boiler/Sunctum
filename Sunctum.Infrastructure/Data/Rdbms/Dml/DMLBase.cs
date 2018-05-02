

using System.Collections.Generic;
using System.Linq;
using Sunctum.Infrastructure.Core;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public abstract class DmlBase : BaseObject, ISqlize
    {
        public static readonly string DELIMITER_SPACE = " ";
        public static readonly string DELIMITER_PARENTHESIS = "(";
        public static readonly string DELIMITER_COMMA = ",";

        protected static void CheckDelimiter(ref string sql)
        {
            if (sql.Count() == 0)
            {
                return;
            }

            if (!char.IsWhiteSpace(sql.Last()) && sql.Last().ToString() != DELIMITER_PARENTHESIS)
            {
                sql += DELIMITER_SPACE;
            }
        }

        public abstract string ToSql();
        public abstract string ToSql(Dictionary<string, int> placeholderNameDictionary);
    }
}
