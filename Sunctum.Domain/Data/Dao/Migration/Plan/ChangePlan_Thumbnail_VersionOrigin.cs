﻿

using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Thumbnail_VersionOrigin : ChangePlanByTable<Thumbnail, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            ThumbnailDao dao = new ThumbnailDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            ThumbnailDao dao = new ThumbnailDao(typeof(VersionOrigin));
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
