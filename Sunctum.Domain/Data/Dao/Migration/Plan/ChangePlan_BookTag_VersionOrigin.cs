

using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_BookTag_VersionOrigin : ChangePlanByTable<BookTag, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new BookTagDao();
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new BookTagDao();
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
