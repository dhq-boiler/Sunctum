

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Exists : IExists
    {
        private Select _Subquery;

        public virtual string KEYWORD { get; } = "EXISTS";

        public string LeftColumnName { get; set; }

        public IRightValue RightValue { get; set; }

        public Select Subquery
        {
            get { return _Subquery; }
            set
            {
                _Subquery = value;
                RightValue = new SubqueryRightValue(_Subquery);
            }
        }

        public Exists(Select subquery)
        {
            Subquery = subquery;
        }

        public string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"{KEYWORD} ({Subquery.ToSql(placeholderNameDictionary)})";
        }
    }
}
