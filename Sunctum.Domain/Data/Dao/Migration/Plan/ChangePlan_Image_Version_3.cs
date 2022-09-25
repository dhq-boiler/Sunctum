

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Conversion;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Image_Version_3 : ChangePlanByTable<Image, Version_3>
    {
        public override void CreateTable(IConnection connection)
        {
            ImageDao dao = new ImageDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            ImageDao dao = new ImageDao(typeof(Version_3));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new ImageDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;

            if (dao.CountAll() > 0)
            {
                dao.Delete(new Dictionary<string, object>());
            }

            dao.UpgradeTable(new VersionChangeUnit(typeof(Version_2), TargetVersion.GetType()));
        }
    }
}
