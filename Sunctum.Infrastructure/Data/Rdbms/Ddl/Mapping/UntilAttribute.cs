

using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class UntilAttribute : Attribute
    {
        public UntilAttribute(Type versionUntil)
        {
            VersionUntil = versionUntil;
        }

        public Type VersionUntil { get; set; }
    }
}
