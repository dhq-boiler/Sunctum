using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;

namespace boilersUpdater.Data.Dao.Migration.Plan
{
    internal class ChangePlan_VC_GitHubReleasesLatest_VersionOrigin : ChangePlanByTable<GitHubReleasesLatest, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new GitHubReleasesLatestDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new GitHubReleasesLatestDao(typeof(VersionOrigin));
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