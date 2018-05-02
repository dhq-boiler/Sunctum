

using System.Collections.Generic;
using Sunctum.Infrastructure.Core;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    public abstract class RightValueImpl : BaseObject, IRightValue
    {
        public abstract string Name { get; set; }

        public abstract object[] Values { get; set; }

        public abstract string ToSql();

        public abstract string ToSql(Dictionary<string, int> placeholderNameDictionary);

        protected string EscapedPlaceholderName
        {
            get
            {
                return Name.Replace('.', '_').ToLower();
            }
        }
    }
}
