
using System;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class JoinOn : DmlBase, ISqlize
    {
        public string LeftOnPropertyName { get; private set; }

        public string RightOnPropertyName { get; private set; }

        public ITable RightTable { get; set; }

        public bool AddPrefix { get; set; }

        public JoinOn(string leftOnPropertyName, string rightOnPropertyName)
        {
            LeftOnPropertyName = leftOnPropertyName;
            RightOnPropertyName = rightOnPropertyName;
            AddPrefix = true;
        }

        public JoinOn(string leftOnPropertyName, string rightOnPropertyName, bool addPrefix)
            : this(leftOnPropertyName, rightOnPropertyName)
        {
            AddPrefix = addPrefix;
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            if (LeftOnPropertyName == null)
                throw new InvalidOperationException();

            if (RightOnPropertyName == null)
                throw new InvalidOperationException();

            if (RightTable == null)
                throw new InvalidOperationException();

            string prefix = (AddPrefix ? RightTable.Name + "." : "");

            return $"ON {LeftOnPropertyName} = {prefix}{RightOnPropertyName}";
        }
    }
}
