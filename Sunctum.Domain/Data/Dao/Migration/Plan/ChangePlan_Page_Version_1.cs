

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Page_Version_1 : ChangePlanByTable<Page, Version_1>
    {
        public override void CreateTable(IConnection connection)
        {
            PageDao dao = new PageDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            PageDao dao = new PageDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new PageDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;

            if (dao.CountAll() > 0)
            {
                dao.Delete(new Dictionary<string, object>());
            }

            dao.UpgradeTable(new VersionChangeUnit(typeof(VersionOrigin), TargetVersion.GetType()));
        }
    }
}
