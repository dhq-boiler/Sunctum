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
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class LibraryInitializingTest : TestSession
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
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
            s_logger.Info("BEGIN AsyncContext.Run");
            AsyncContext.Run(async () =>
            {
                s_logger.Info("BEGIN s_libManager.Initialize");
                await s_libManager.Initialize();
                s_logger.Info("END s_libManager.Initialize");
            });
            s_logger.Info("END AsyncContext.Run");

            //try
            //{
            //    using (var fs = File.Open(_filePath, FileMode.CreateNew))
            //    { }
            //}
            //catch (IOException e)
            //{
            //    Assert.Pass($"{_filePath} exists");
            //}
            //Assert.Fail($"{_filePath} doesn't exist");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            s_libManager.Dispose();

            var mwvm = Container.Resolve<IMainWindowViewModel>();
            mwvm.Close();

            GC.Collect();

            //if (File.Exists(_filePath))
            //{
            //    File.Delete(_filePath);
            //}
        }

        public override string GetTestDirectory()
        {
            return TestContext.CurrentContext.TestDirectory;
        }
    }
}
