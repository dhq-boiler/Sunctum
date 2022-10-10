

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using System;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_KeyValue_VersionOrigin : ChangePlan<KeyValue, VersionOrigin>
    {
        public ChangePlan_KeyValue_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("KeyValue", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }
        public override void CreateTable(IConnection connection)
        {
            var dao = new KeyValueDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new KeyValueDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);

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
