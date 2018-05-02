

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class GroupBy : DmlBase, ISqlize
    {
        private string[] _columnNames;

        public GroupBy(params string[] columnNames)
        {
            _columnNames = columnNames;
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = $"GROUP BY";

            foreach (var columnName in _columnNames)
            {
                CheckDelimiter(ref sql);
                sql += columnName;
            }

            return sql;
        }
    }
}
