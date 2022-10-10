

using Homura.ORM;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Book_Version_3 : ChangePlan<Book, Version_3>
    {
        public ChangePlan_Book_Version_3(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("Book_3", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override void CreateTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new BookDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;

            if (dao.CountAll() > 0)
            {
                dao.Delete(new Dictionary<string, object>());
                ++ModifiedCount;
            }

            dao.UpgradeTable(new VersionChangeUnit(typeof(Version_2), TargetVersion.GetType()), Mode);
            ++ModifiedCount;
        }
    }
}
