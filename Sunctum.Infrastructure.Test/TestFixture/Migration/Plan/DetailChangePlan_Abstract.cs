﻿

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using Sunctum.Infrastructure.Test.TestFixture.Dao;
using Sunctum.Infrastructure.Test.TestFixture.Entity;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal abstract class DetailChangePlan_Abstract<V> : ChangePlanByTable<Detail, V> where V : VersionOrigin
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new DetailDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new DetailDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }
    }
}
