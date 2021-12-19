

using Homura.ORM;
using NUnit.Framework;
using Sunctum.Domain.Data.Dao;
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
    public class PageDaoTest
    {
        //private static TestBootstrapper s_bootstrapper;
        private ILibrary _libManager;
        private string _filePath;
        private string _dirPath;

        [SetUp]
        public void SetUp()
        {
            _dirPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PageDaoTest");
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

            //_libManager = s_bootstrapper.Get<ILibrary>();
            _libManager.Initialize().Wait();
            _libManager.Load().Wait();
        }

        [Test]
        public void UpdateNoChangeTest()
        {
            var id = new Guid("66F5EC93-FD2C-4DCC-A76B-5FE3D00C6E58");
            var imageid = new Guid("83506829-1661-4329-8C43-8AE858F00F02");
            var bookid = new Guid("9277ACAF-AB93-47B2-BE04-92566DE6F190");
            var pageindex = int.MaxValue;
            var title = "Inserted page";

            var page = new Page();
            page.ID = id;
            page.ImageID = imageid;
            page.BookID = bookid;
            page.PageIndex = pageindex;
            page.Title = title;

            PageDao dao = new PageDao();
            dao.Insert(page);

            var records = dao.FindBy(new Dictionary<string, object>() { { "ID", id } });

            Assert.That(records.Count(), Is.EqualTo(1));
            var record = records.Single();
            Assert.That(record.ID, Is.EqualTo(id));
            Assert.That(record.ImageID, Is.EqualTo(imageid));
            Assert.That(record.BookID, Is.EqualTo(bookid));
            Assert.That(record.PageIndex, Is.EqualTo(pageindex));
            Assert.That(record.Title, Is.EqualTo(title));

            dao.Update(page);

            records = dao.FindBy(new Dictionary<string, object>() { { "ID", id } });

            Assert.That(records.Count(), Is.EqualTo(1));
            record = records.Single();
            Assert.That(record.ID, Is.EqualTo(id));
            Assert.That(record.ImageID, Is.EqualTo(imageid));
            Assert.That(record.BookID, Is.EqualTo(bookid));
            Assert.That(record.PageIndex, Is.EqualTo(pageindex));
            Assert.That(record.Title, Is.EqualTo(title));
        }

        [Test]
        public void UpdatePageIndexOnlyTest()
        {
            var id = new Guid("66F5EC93-FD2C-4DCC-A76B-5FE3D00C6E58");
            var imageid = new Guid("83506829-1661-4329-8C43-8AE858F00F02");
            var bookid = new Guid("9277ACAF-AB93-47B2-BE04-92566DE6F190");
            var pageindex = int.MaxValue;
            var title = "Inserted page";

            var page = new Page();
            page.ID = id;
            page.ImageID = imageid;
            page.BookID = bookid;
            page.PageIndex = pageindex;
            page.Title = title;

            PageDao dao = new PageDao();
            dao.Insert(page);

            var records = dao.FindBy(new Dictionary<string, object>() { { "ID", id } });

            Assert.That(records.Count(), Is.EqualTo(1));
            var record = records.Single();
            Assert.That(record.ID, Is.EqualTo(id));
            Assert.That(record.ImageID, Is.EqualTo(imageid));
            Assert.That(record.BookID, Is.EqualTo(bookid));
            Assert.That(record.PageIndex, Is.EqualTo(pageindex));
            Assert.That(record.Title, Is.EqualTo(title));

            //Change
            page.PageIndex = 1;

            dao.Update(page);

            records = dao.FindBy(new Dictionary<string, object>() { { "ID", id } });

            Assert.That(records.Count(), Is.EqualTo(1));
            record = records.Single();
            Assert.That(record.ID, Is.EqualTo(id));
            Assert.That(record.ImageID, Is.EqualTo(imageid));
            Assert.That(record.BookID, Is.EqualTo(bookid));
            Assert.That(record.PageIndex, Is.EqualTo(1));
            Assert.That(record.Title, Is.EqualTo(title));
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
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
