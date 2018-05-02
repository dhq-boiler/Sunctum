

using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SinceAttribute : Attribute
    {
        public SinceAttribute(Type versionSince)
        {
            VersionSince = versionSince;
        }

        public Type VersionSince { get; set; }
    }
}
