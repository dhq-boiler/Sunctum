﻿

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class GammaChangePlan_VersionOrigin : GammaChangePlan_Abstract<VersionOrigin>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
