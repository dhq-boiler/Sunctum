

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_AppDb_Statistics_VersionOrigin : ChangePlanByTable<Statistics, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            StatisticsDao dao = new StatisticsDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            StatisticsDao dao = new StatisticsDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}
