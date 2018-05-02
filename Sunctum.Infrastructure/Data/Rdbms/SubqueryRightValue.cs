

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sunctum.Infrastructure.Data.Rdbms.Dml;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    public class SubqueryRightValue : RightValueImpl
    {
        public override string Name { get; set; }

        public override object[] Values { get; set; }

        public Select Subquery { get; set; }

        public SubqueryRightValue(Select subquery)
        {
            Contract.Assert(subquery != null);
            Subquery = subquery;
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            return $"({Subquery.ToSql(placeholderNameDictionary)})";
        }
    }
}
