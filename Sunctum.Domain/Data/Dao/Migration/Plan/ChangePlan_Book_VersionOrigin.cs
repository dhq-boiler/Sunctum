﻿

using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using Sunctum.Domain.Models;

namespace Sunctum.Domain.Data.Dao.Migration.Plan
{
    internal class ChangePlan_Book_VersionOrigin : ChangePlan<Book, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            BookDao dao = new BookDao(typeof(VersionOrigin));
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
