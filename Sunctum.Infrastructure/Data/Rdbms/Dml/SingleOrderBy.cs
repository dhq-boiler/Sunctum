

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    internal class SingleOrderBy : ISqlize
    {
        public SingleOrderBy(string columnName, Ordering ordering = Ordering.Ascending)
        {
            ColumnName = columnName;
            Ordering = ordering;
        }

        public string ColumnName { get; set; }

        public Ordering Ordering { get; set; }

        public string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = $"{ColumnName}";
            if (Ordering != Ordering.Ascending)
            {
                sql += $" {OrderingToString(Ordering)}";
            }
            return sql;
        }

        private static string OrderingToString(Ordering order)
        {
            switch (order)
            {
                case Ordering.Ascending:
                    return "asc";
                case Ordering.Descending:
                    return "desc";
                default:
                    return "UNKNOWN";
            }
        }
    }
}
