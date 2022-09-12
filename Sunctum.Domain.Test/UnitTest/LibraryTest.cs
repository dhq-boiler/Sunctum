﻿using Homura.ORM;
using Nito.AsyncEx;
using NUnit.Framework;
using Prism.Ioc;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using Sunctum.Domain.ViewModels;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    public class LibraryTest : TestSession
    {
        private string _filePath;
        private string _dirPath;
        private string _dataPath;
        private ILibrary _libManager;

        [Test]
        public void 検索中インポート()
        {
            _dirPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PageOrderingTest");
            _filePath = Path.Combine(_dirPath, "library.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            if (Directory.Exists(_dirPath))
            {
                Directory.Delete(_dirPath, true);
            }

            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }

            _dataPath = Path.Combine(_dirPath, "data");

            _libManager = Container.Resolve<ILibrary>();

            var mwvm = Container.Resolve<IMainWindowViewModel>();
            mwvm.ManageAppDB();
            mwvm.ManageVcDB();
            mwvm.InitializeWindowComponent();
            var home = mwvm.HomeDocumentViewModel;
            home.BookCabinet = _libManager.CreateBookStorage();
            home.BookCabinet.Search("minecraft");

            AsyncContext.Run(async () =>
            {
                await mwvm.Initialize(true, false);
                await _libManager.ImportAsync(new string[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minecraft_screenshots") });
            });

            _libManager.TaskManager.WaitUntilProcessAll();

            Assert.That(home.BookCabinet.OnStage[0], Has.Property("Title").Contains("minecraft"));
        }

        [TearDown]
        public void TearDown()
        {
            var mwvm = Container.Resolve<IMainWindowViewModel>();
            mwvm.Close();

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            if (Directory.Exists(_dataPath))
            {
                Directory.Delete(_dataPath, true);
            }

            if (Directory.Exists(_dirPath + "\\cache"))
            {
                Directory.Delete(_dirPath + "\\cache", true);
            }

            if (Directory.Exists(_dirPath))
            {
                Directory.Delete(_dirPath, true);
            }
        }

        public override string GetTestDirectory()
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "LibraryTest");
        }
    }
}