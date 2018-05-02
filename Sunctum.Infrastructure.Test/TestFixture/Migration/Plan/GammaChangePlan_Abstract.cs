

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using Sunctum.Infrastructure.Test.TestFixture.Dao;
using Sunctum.Infrastructure.Test.TestFixture.Entity;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class GammaChangePlan_Abstract<V> : ChangePlanByTable<Gamma, V> where V : VersionOrigin
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new GammaDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
        }
    }
}
