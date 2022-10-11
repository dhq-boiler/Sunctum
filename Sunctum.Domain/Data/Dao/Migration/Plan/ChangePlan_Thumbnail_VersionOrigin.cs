

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Thumbnail_VersionOrigin : ChangePlan<Thumbnail, VersionOrigin>
    {
        public ChangePlan_Thumbnail_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("Thumbnail", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override async Task CreateTable(IConnection connection)
        {
            ThumbnailDao dao = new ThumbnailDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            ThumbnailDao dao = new ThumbnailDao(typeof(VersionOrigin));
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
