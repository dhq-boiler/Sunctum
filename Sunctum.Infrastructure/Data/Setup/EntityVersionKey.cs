

using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;

namespace Sunctum.Infrastructure.Data.Setup
{
    public class EntityVersionKey : BaseObject
    {
        public Type TargetEntityType { get; set; }

        public VersionOrigin TargetVersion { get; set; }

        public EntityVersionKey(Type targetEntityType, VersionOrigin targetVersion)
        {
            TargetEntityType = targetEntityType;
            TargetVersion = targetVersion;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EntityVersionKey)) return false;

            var o = obj as EntityVersionKey;
            return TargetEntityType.FullName.Equals(o.TargetEntityType.FullName)
                && TargetVersion.GetType().FullName.Equals(o.TargetVersion.GetType().FullName);
        }

        public override int GetHashCode()
        {
            return TargetEntityType.FullName.GetHashCode()
                 ^ TargetVersion.GetType().FullName.GetHashCode();
        }
    }
}
