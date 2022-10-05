

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Tag_VersionOrigin : ChangePlan<Tag, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            TagDao dao = new TagDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            TagDao dao = new TagDao(typeof(VersionOrigin));
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
