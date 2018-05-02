﻿

using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_ImageTag_VersionOrigin : ChangePlanByTable<ImageTag, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            ImageTagDao dao = new ImageTagDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            ImageTagDao dao = new ImageTagDao(typeof(VersionOrigin));
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
