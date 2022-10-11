

using Homura.ORM;
using Homura.ORM.Migration;
using Homura.ORM.Setup;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Book_Version_3 : ChangePlan<Book, Version_3>
    {
        public ChangePlan_Book_Version_3(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("Book_3", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override async Task CreateTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            await dao.DropTableAsync();
            ++ModifiedCount;
        }

        public override async Task UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new BookDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;

            if (await dao.CountAllAsync() > 0)
            {
                await dao.DeleteAsync(new Dictionary<string, object>());
                ++ModifiedCount;
            }

            await dao.UpgradeTableAsync(new VersionChangeUnit(typeof(Version_2), TargetVersion.GetType()), Mode);
            ++ModifiedCount;
        }
    }
}
