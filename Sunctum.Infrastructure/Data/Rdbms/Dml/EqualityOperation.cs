
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    internal class EqualityOperation : IComparisonOperation
    {
        public string LeftColumnName { get; set; }

        public IRightValue RightValue { get; set; }

        public EqualityOperation(string leftColumnName, IRightValue rightValue)
        {
            LeftColumnName = leftColumnName;
            RightValue = rightValue;
        }

        public string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{LeftColumnName} = {RightValue.ToSql(placeholderNameDictionary)}";
        }
    }
}
