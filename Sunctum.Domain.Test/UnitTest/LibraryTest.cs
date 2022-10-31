using Homura.ORM;
using NUnit.Framework;
using Prism.Common;
using Prism.Ioc;
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
    public class LibraryTest : TestSession
    {
        private string _dirPath;
        private string _dataPath;
        private ILibrary _libManager;
        private static Guid _instanceId = Guid.NewGuid();

        [SetUp]
        public void _SetUp()
        {
            _dirPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "LibraryTest");

            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }

            _dataPath = Path.Combine(_dirPath, "data");

            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        [Retry(3)]
        [Test]
        public async Task 検索中インポート()
        {
            var mwvm = Container.Resolve<IMainWindowViewModel>();
            await mwvm.ManageAppDB().ConfigureAwait(false);
            await mwvm.ManageVcDB().ConfigureAwait(false);
            mwvm.InitializeWindowComponent();

            _libManager = Container.Resolve<ILibrary>();
            var home = mwvm.HomeDocumentViewModel;
            home.BookCabinet = _libManager.CreateBookStorage();
            await home.BookCabinet.Search("minecraft").ConfigureAwait(false);

            mwvm.Initialize1stPhase(true);
            await mwvm.Initialize3rdPhase().ConfigureAwait(false);
            await _libManager.ImportAsync(new string[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minecraft_screenshots") }).ConfigureAwait(false);

            Assert.That(home.BookCabinet.OnStage[0], Has.Property("Title").Contains("minecraft"));
        }

        [TearDown]
        public void TearDown()
        {
            var mwvm = Container.Resolve<IMainWindowViewModel>();
            mwvm.Close();
            mwvm.Dispose();

            ConnectionManager.DisposeDebris(Guid.Parse("9056E8CF-745D-4BCC-AEB9-14B1D1B40F37"));
            ConnectionManager.DisposeAllDebris();

            GC.Collect();

            if (Directory.Exists(_dataPath))
            {
                Directory.Delete(_dataPath, true);
            }

            if (Directory.Exists(_dirPath + "\\cache"))
            {
                Directory.Delete(_dirPath + "\\cache", true);
            }

            //if (Directory.Exists(_dirPath))
            //{
            //    Directory.Delete(_dirPath, true);
            //}
        }

        public override string GetTestDirectory()
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "LibraryTest");
        }
    }
}
