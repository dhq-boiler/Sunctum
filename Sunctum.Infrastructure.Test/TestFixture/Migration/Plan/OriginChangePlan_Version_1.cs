

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using Sunctum.Infrastructure.Test.TestFixture.Dao;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class OriginChangePlan_Version_1 : OriginChangePlan_Abstract<Version_1>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new OriginDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;

            dao.CreateTableIfNotExists();

            if (dao.CountAll() > 0)
            {
                dao.Delete(new Dictionary<string, object>());
            }

            dao.UpgradeTable(new VersionChangeUnit(typeof(VersionOrigin), TargetVersion.GetType()));
        }
    }
}
