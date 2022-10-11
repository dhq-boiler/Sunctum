using Homura.ORM.Mapping;
using Homura.ORM;
using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using Sunctum.Domain.Models;
using System.Collections.Generic;
using Homura.ORM.Setup;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Author_Version_1 : ChangePlan<Author, Version_1>
    {
        public ChangePlan_Author_Version_1(VersioningMode mode, MigrationAction migrationAction = MigrationAction.NotSpecified) : base("Author_1", PostMigrationVerification.TableExists, mode, migrationAction)
        {
        }

        public override async Task CreateTable(IConnection connection)
        {
            AuthorDao dao = new AuthorDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            await dao.CreateTableIfNotExistsAsync();
            ++ModifiedCount;
            await dao.CreateIndexIfNotExistsAsync();
            ++ModifiedCount;
        }

        public override async Task DropTable(IConnection connection)
        {
            AuthorDao dao = new AuthorDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            await dao.DropTableAsync();
            ++ModifiedCount;
        }

        public override async Task UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new AuthorDao(TargetVersion.GetType());
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

            await dao.UpgradeTableAsync(new VersionChangeUnit(typeof(VersionOrigin), TargetVersion.GetType()), Mode);
            ++ModifiedCount;
        }
    }
}