
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class In : IIn
    {
        public virtual string KEYWORD { get; } = "IN";

        public string LeftColumnName { get; set; }

        public IRightValue RightValue { get; set; }

        public In(string leftColumnName, IRightValue rightValue)
        {
            LeftColumnName = leftColumnName;
            RightValue = rightValue;
        }

        public In(string columnName, object[] parameter)
        {
            LeftColumnName = columnName;
            RightValue = new MultiplePlaceholderRightValue(columnName, parameter);
        }

        public In(string columnName, Select subquery)
        {
            LeftColumnName = columnName;
            RightValue = new SubqueryRightValue(subquery);
        }

        public string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{LeftColumnName} {KEYWORD} {RightValue.ToSql(placeholderNameDictionary)}";
        }
    }
}
