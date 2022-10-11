

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_KeyValue_VersionOrigin : ChangePlan<KeyValue, VersionOrigin>
    {
        public ChangePlan_KeyValue_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("KeyValue", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }
        public override async Task CreateTable(IConnection connection)
        {
            var dao = new KeyValueDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            var dao = new KeyValueDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            await dao.DropTableAsync();
            ++ModifiedCount;
        }

        public override async Task UpgradeToTargetVersion(IConnection connection)
        {
            await CreateTable(connection);

            var dao = new KeyValueDao();
            dao.CurrentConnection = connection;
            dao.Insert(new KeyValue()
            {
                Key = "LibraryID",
                Value = Guid.NewGuid().ToString(),
            });
        }
    }
}
