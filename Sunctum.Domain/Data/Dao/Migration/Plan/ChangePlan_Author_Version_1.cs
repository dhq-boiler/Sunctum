using Homura.ORM.Mapping;
using Homura.ORM;
using Homura.ORM.Migration;
using Sunctum.Domain.Models.Conversion;
using Sunctum.Domain.Models;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Author_Version_1 : ChangePlanByTable<Author, Version_1>
    {
        public override void CreateTable(IConnection connection)
        {
            AuthorDao dao = new AuthorDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            AuthorDao dao = new AuthorDao(typeof(Version_1));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new AuthorDao(TargetVersion.GetType());
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

            dao.UpgradeTable(new VersionChangeUnit(typeof(VersionOrigin), TargetVersion.GetType()));
            ++ModifiedCount;
        }
    }
}