using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System.Threading.Tasks;
using System;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_DoNothing : ChangePlan<Dummy, VersionOrigin>
    {
        public ChangePlan_DoNothing(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("DoNothing", PostMigrationVerification.DoNothing, mode, migrationAction)
        {
        }

        public override async Task UpgradeToTargetVersion(IConnection connection)
        {
            //DoNothing
        }
    }
}