

using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Infrastructure.Data.Setup
{
    public class VersionKey : BaseObject
    {
        public VersionOrigin TargetVersion { get; set; }

        public VersionKey(VersionOrigin targetVersion)
        {
            TargetVersion = targetVersion;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VersionKey)) return false;

            var o = obj as VersionKey;
            return TargetVersion.GetType().FullName.Equals(o.TargetVersion.GetType().FullName);
        }

        public override int GetHashCode()
        {
            return TargetVersion.GetType().FullName.GetHashCode();
        }
    }
}
