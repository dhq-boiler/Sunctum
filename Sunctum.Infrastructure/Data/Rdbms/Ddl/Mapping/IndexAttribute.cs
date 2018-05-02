

using System;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public IEnumerable<string> PropertyNames { get; private set; }

        public IndexAttribute()
        { }

        public IndexAttribute(params string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
    }
}
