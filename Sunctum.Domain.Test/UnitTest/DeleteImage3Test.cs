using Homura.ORM;
using Homura.ORM.Setup;
using NUnit.Framework;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration.Plan;
using Sunctum.Domain.Models.Conversion;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Test.UnitTest
{
    [TestFixture]
    public class DeleteImage3Test
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "BookFacadeTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            DataVersionManager dvManager = new DataVersionManager();
            dvManager.CurrentConnection = ConnectionManager.DefaultConnection;
            dvManager.Mode = VersioningStrategy.ByTick;
            dvManager.RegisterChangePlan(new ChangePlan_VersionOrigin());
            dvManager.RegisterChangePlan(new ChangePlan_Version_1());
            dvManager.RegisterChangePlan(new ChangePlan_Version_2());
            dvManager.RegisterChangePlan(new ChangePlan_Version_3());
            dvManager.RegisterChangePlan(new ChangePlan_Version_4());
            dvManager.RegisterChangePlan(new ChangePlan_Version_5());
            dvManager.RegisterChangePlan(new ChangePlan_Version_6());
            dvManager.RegisterChangePlan(new ChangePlan_Version_7());
            dvManager.UpgradeToTargetVersion();
        }

        [Test]
        public void Image_3テーブルを全削除()
        {
            var dao = new ImageDao(typeof(Version_3));
            dao.Delete(new Dictionary<string, object>());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ConnectionManager.DisposeAllDebris();
        }
    }
}
