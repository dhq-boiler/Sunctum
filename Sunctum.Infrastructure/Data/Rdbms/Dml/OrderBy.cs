

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class OrderBy : DmlBase, IOrderBy, ISqlize
    {
        private List<SingleOrderBy> _orderByConditions;

        public OrderBy()
        {
            _orderByConditions = new List<SingleOrderBy>();
        }

        public void Add(string columnName, Ordering ordering)
        {
            _orderByConditions.Add(new SingleOrderBy(columnName, ordering));
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = "ORDER BY";
            foreach (var condition in _orderByConditions)
            {
                CheckDelimiter(ref sql);
                sql += condition.ToSql(placeholderNameDictionary);
            }

            return sql;
        }
    }
}
