

using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Migration;
using Sunctum.Infrastructure.Test.TestFixture.Dao;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Test.TestFixture.Migration.Plan
{
    internal class HeaderChangePlan_Version_2 : HeaderChangePlan_Abstract<Version_2>
    {
        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new HeaderDao(TargetVersion.GetType());
            dao.CurrentConnection = connection;

            dao.CreateTableIfNotExists();

            if (dao.CountAll() > 0)
            {
                dao.Delete(new Dictionary<string, object>());
            }

            dao.UpgradeTable(new VersionChangeUnit(typeof(Version_1), TargetVersion.GetType()));
        }
    }
}
