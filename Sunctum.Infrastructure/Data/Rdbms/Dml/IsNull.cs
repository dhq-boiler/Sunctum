

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class IsNull : IIsNull
    {
        public virtual string KEYWORD { get; } = "IS NULL";

        public string LeftColumnName { get; set; }

        /// <summary>
        /// be ignored
        /// </summary>
        public IRightValue RightValue { get; set; }

        public IsNull(string leftColumnName)
        {
            LeftColumnName = leftColumnName;
        }

        public string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{LeftColumnName} {KEYWORD}";
        }
    }
}
