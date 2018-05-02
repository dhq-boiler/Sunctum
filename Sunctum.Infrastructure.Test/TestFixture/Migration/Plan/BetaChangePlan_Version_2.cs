
using Sunctum.Infrastructure.Data.Rdbms;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class BetaChangePlan_Version_2 : BetaChangePlan_Abstract<Version_2>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
