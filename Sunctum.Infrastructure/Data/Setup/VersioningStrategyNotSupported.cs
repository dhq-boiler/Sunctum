﻿

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using System;

namespace Sunctum.Infrastructure.Data.Setup
{
    internal class VersioningStrategyNotSupported : VersioningStrategy
    {
        internal override IVersionChangePlan GetPlan(VersionOrigin targetVersion)
        {
            throw new NotSupportedException();
        }

        internal override IEntityVersionChangePlan GetPlan(Type targetEntityType, VersionOrigin targetVersion)
        {
            throw new NotSupportedException();
        }

        internal override void RegisterChangePlan(IEntityVersionChangePlan plan)
        {
            throw new NotSupportedException();
        }

        internal override void RegisterChangePlan(IVersionChangePlan plan)
        {
            throw new NotSupportedException();
        }

        internal override void Reset()
        {
            //Do nothing
        }

        internal override void UnregisterChangePlan(VersionOrigin targetVersion)
        {
            throw new NotSupportedException();
        }

        internal override void UnregisterChangePlan(Type targetEntityType, VersionOrigin targetVersion)
        {
            throw new NotSupportedException();
        }

        internal override void UpgradeToTargetVersion(IConnection connection)
        {
            throw new NotSupportedException();
        }
    }
}
