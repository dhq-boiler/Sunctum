

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_DirectoryNameParser_VersionOrigin : ChangePlan<DirectoryNameParser, VersionOrigin>
    {
        public ChangePlan_DirectoryNameParser_VersionOrigin(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("DirectoryNameParser", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override void CreateTable(IConnection connection)
        {
            var dao = new DirectoryNameParserDao();
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new DirectoryNameParserDao();
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
