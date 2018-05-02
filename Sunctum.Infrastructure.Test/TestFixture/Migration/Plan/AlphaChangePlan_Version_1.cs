
using Sunctum.Infrastructure.Data.Rdbms;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class AlphaChangePlan_Version_1 : AlphaChangePlan_Abstract<Version_1>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
