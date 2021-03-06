

using Homura.ORM;
using NUnit.Framework;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using System.Data.SQLite;
using System.IO;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class LibraryInitializingTest
    {
        private static TestBootstrapper s_bootstrapper;
        private static ILibrary s_libManager;
        private string _filePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "LibraryInitializingTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            s_bootstrapper = new TestBootstrapper($"Data Source={_filePath}");
            s_bootstrapper.Run();

            var config = new Configuration();
            config.WorkingDirectory = TestContext.CurrentContext.TestDirectory;
            Configuration.ApplicationConfiguration = config;

            s_libManager = s_bootstrapper.Get<ILibrary>();
        }

        [Test]
        public void InitializeLibraryTest()
        {
            s_libManager.Initialize().Wait();

            Assert.That(File.Exists(_filePath), Is.True);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
