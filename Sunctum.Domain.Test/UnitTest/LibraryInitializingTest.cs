using Homura.ORM;
using Nito.AsyncEx;
using NLog;
using NUnit.Framework;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using Sunctum.Domain.ViewModels;
using System;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class LibraryInitializingTest : TestSession
    {
        private static ILibrary s_libManager;
        private string _filePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "LibraryInitializingTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            s_libManager = Container.Resolve<ILibrary>();
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

        public override string GetTestDirectory()
        {
            return TestContext.CurrentContext.TestDirectory;
        }
    }
}
