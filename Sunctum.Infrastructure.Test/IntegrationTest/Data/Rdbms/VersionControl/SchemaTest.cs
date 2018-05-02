

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Setup;
using Sunctum.Infrastructure.Test.TestFixture.Dao;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Sunctum.Infrastructure.Test.IntegrationTest.Data.Rdbms.VersionControl
{
    [Category("Infrastructure")]
    [Category("IntegrationTest")]
    [TestFixture]
    public class SchemaTest
    {
        private string _filePath;

        [SetUp]
        public void Initialize()
        {
            _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TableNameTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));
        }

        [Test]
        public void CreateTable_Specified_VersionOrigin()
        {
            var svManager = new DataVersionManager();
            svManager.SetDefault();

            //Create VersionOrigin
            var dao = new OriginDao(typeof(VersionOrigin));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            dao.CreateTableIfNotExists();

            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(3));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
            }
        }

        [Test]
        public void CreateTable_DaoUseDefaultConstructor_VersionOrigin()
        {
            var svManager = new DataVersionManager();
            svManager.SetDefault();

            //Create VersionOrigin
            var dao = new OriginDao();
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            dao.CreateTableIfNotExists();

            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(3));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
            }
        }

        [Test]
        public void CreateTable_Specified_Version_1()
        {
            var svManager = new DataVersionManager();
            svManager.SetDefault();

            //Create Version_1
            var dao = new OriginDao(typeof(Version_1));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            dao.CreateTableIfNotExists();

            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(4));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item3"));
            }
        }

        [Test]
        public void CreateTable_Specified_VersionOrigin_Version_1()
        {
            var svManager = new DataVersionManager();
            svManager.SetDefault();

            //1. Create VersionOrigin
            var dao = new OriginDao(typeof(VersionOrigin));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            dao.CreateTableIfNotExists();

            //check Header(VersionOrigin)
            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(3));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
            }

            //Create Version_1
            dao = new OriginDao(typeof(Version_1));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            dao.CreateTableIfNotExists();

            //check Header(VersionOrigin)
            dao = new OriginDao(typeof(VersionOrigin));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(3));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
            }

            //check Header(Version_1)
            dao = new OriginDao(typeof(Version_1));
            dao.CurrentConnection = ConnectionManager.DefaultConnection;
            using (var conn = new SQLiteConnection($"Data Source={_filePath}"))
            {
                conn.Open();

                Assert.That(conn.GetTableNames(), Has.Exactly(1).EqualTo(dao.TableName));

                var columnNames = conn.GetColumnNames(dao.TableName);
                Assert.That(columnNames.Count(), Is.EqualTo(4));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Id"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item1"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item2"));
                Assert.That(columnNames, Has.Exactly(1).EqualTo("Item3"));
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
