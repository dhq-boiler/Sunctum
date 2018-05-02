
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class ColumnNameDirectValuePair : ColumnNameBindValuePair
    {
        public ColumnNameDirectValuePair(string columnName, object value) : base(columnName, value)
        {
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{ColumnName} = {Value}";
        }
    }
}
