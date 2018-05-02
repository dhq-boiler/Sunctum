
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class ColumnNameBindValuePair : ISqlize
    {
        public PlaceholderRightValue Parameter { get; private set; }

        public ColumnNameBindValuePair(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
            Parameter = new PlaceholderRightValue(columnName.ToLower(), value);
        }

        public string ColumnName { get; set; }

        public object Value { get; set; }

        public virtual string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public virtual string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{ColumnName} = {Parameter.ToSql(placeholderNameDictionary)}";
        }
    }
}
