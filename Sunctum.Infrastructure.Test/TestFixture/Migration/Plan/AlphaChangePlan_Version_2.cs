﻿
using Sunctum.Infrastructure.Data.Rdbms;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class AlphaChangePlan_Version_2 : AlphaChangePlan_Abstract<Version_2>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
