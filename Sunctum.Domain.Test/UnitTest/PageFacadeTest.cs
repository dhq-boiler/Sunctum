

using Homura.ORM;
using Nito.AsyncEx;
using NUnit.Framework;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class PageFacadeTest
    {
        //private static TestBootstrapper s_bootstrapper;
        private string _filePath;
        private string _dirPath;
        private string _dataPath;
        private ILibrary _libManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dirPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PageFacadeTest");
            _filePath = Path.Combine(_dirPath, "library.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            //s_bootstrapper = new TestBootstrapper($"Data Source={_filePath}");
            //s_bootstrapper.Run();

            if (Directory.Exists(_dirPath))
            {
                Directory.Delete(_dirPath, true);
            }

            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }


            var config = new Configuration();
            config.WorkingDirectory = _dirPath;
            Configuration.ApplicationConfiguration = config;

            _dataPath = Path.Combine(_dirPath, "data");

            //_libManager = s_bootstrapper.Get<ILibrary>();

            AsyncContext.Run(async () =>
            {
                await _libManager.Initialize();
                await _libManager.Load();
                await _libManager.ImportAsync(new string[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minecraft_screenshots") });
            });
        }

        [Test]
        public void UpdatePageIndexTest()
        {
            var book = _libManager.BookSource.First();

            Assert.That(book.Contents.Count, Is.EqualTo(26));

            var bookContents = book.Contents;

            Console.WriteLine("==========BEFORE==========");
            Dictionary<Guid, Guid> imageIds = new Dictionary<Guid, Guid>();
            foreach (var page in bookContents)
            {
                Console.WriteLine(page.ToString());
                Assert.That(page.BookID, Is.EqualTo(book.ID));

                imageIds.Add(page.ID, page.ImageID);

                page.PageIndex = page.PageIndex + 10000;
                PageFacade.Update(page);
            }

            AsyncContext.Run(async () =>
            {
                //再読み込み
                await _libManager.Load();
            });

            var reload_book = _libManager.BookSource.First();

            //ブックの読み込み
            _libManager.RunFillContentsWithImage(reload_book);

            Assert.That(reload_book.Contents.Count, Is.EqualTo(26));

            var reload_bookContents = reload_book.Contents;

            Console.WriteLine("==========AFTER==========");

            foreach (var page in reload_bookContents)
            {
                Console.WriteLine(page.ToString());
                Assert.That(page.BookID, Is.EqualTo(book.ID));
                Assert.That(page.ImageID, Is.EqualTo(imageIds[page.ID]));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
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
    }
}
