

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Limit : DmlBase, ILimit
    {
        private int _beginIndex = -1;
        private int _rowCount;

        public Limit(int rowCount)
        {
            _rowCount = rowCount;
        }

        public Limit(int rowCount, int beginIndex) : this(rowCount)
        {
            _beginIndex = beginIndex;
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = "LIMIT";
            CheckDelimiter(ref sql);
            sql += $"{_rowCount}";
            if (_beginIndex >= 0)
            {
                CheckDelimiter(ref sql);
                sql += $"OFFSET {_beginIndex}";
            }
            return sql;
        }
    }
}
