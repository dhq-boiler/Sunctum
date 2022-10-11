

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Tag_VersionOrigin : ChangePlan<Tag, VersionOrigin>
    {
        public ChangePlan_Tag_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("Tag", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override async Task CreateTable(IConnection connection)
        {
            TagDao dao = new TagDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            TagDao dao = new TagDao(typeof(VersionOrigin));
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
