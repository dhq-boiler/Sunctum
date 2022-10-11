

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_ImageTag_VersionOrigin : ChangePlan<ImageTag, VersionOrigin>
    {
        public ChangePlan_ImageTag_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("ImageTag", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override async Task CreateTable(IConnection connection)
        {
            ImageTagDao dao = new ImageTagDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            ImageTagDao dao = new ImageTagDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.DropTableAsync();
            ++ModifiedCount;
        }

        public override async Task UpgradeToTargetVersion(IConnection connection)
        {
            await CreateTable(connection);
        }
    }
}
