

using Homura.ORM;
using Nito.AsyncEx;
using NUnit.Framework;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    public class PageOrderingTest : TestSession
    {
        private string _filePath;
        private string _dirPath;
        private string _dataPath;
        private ILibrary _libManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
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

            AsyncContext.Run(async () =>
            {
                await _libManager.Initialize();
                await _libManager.Load();
                await _libManager.ImportAsync(new string[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "minecraft_screenshots") });
            });
        }

        [Test]
        public void ChangingPageOrderTest()
        {
            var book = _libManager.BookSource.First();

            //ブックの読み込み
            _libManager.RunFillContentsWithImage(book);

            Assert.That(book.Contents.Count, Is.EqualTo(26));

            var first = book.Contents[0];
            var second = book.Contents[1];


            Console.WriteLine("==========BEFORE==========");
            Console.WriteLine(first);
            Console.WriteLine(second);

            Guid first_guid = first.ID;
            Guid second_guid = second.ID;

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(first.Title, Is.EqualTo("2015-03-05_02.46.55"));
            Assert.That(second.Title, Is.EqualTo("2015-03-05_03.28.02"));

            //1番目と2番目のページを入れ替え（1番目を後ろに）
            var newOrderedBook = _libManager.OrderForward(first, book);

            AsyncContext.Run(async () =>
            {
                //入れ替えた結果をDBに書き込み
                await _libManager.SaveBookContentsOrder(newOrderedBook);

                //再読み込み
                await _libManager.Load();
            });

            book = _libManager.BookSource.First();

            //ブックの読み込み
            _libManager.RunFillContentsWithImage(book);

            Assert.That(book.Contents.Count, Is.EqualTo(26));

            var after_first = book.Contents[0];
            var after_second = book.Contents[1];

            Console.WriteLine("==========AFTER==========");
            Console.WriteLine(after_first);
            Console.WriteLine(after_second);

            Assert.That(after_first, Is.Not.Null);
            Assert.That(after_second, Is.Not.Null);

            Assert.That(after_first.PageIndex, Is.EqualTo(1));
            Assert.That(after_second.PageIndex, Is.EqualTo(2));
            Assert.That(after_first.Title, Is.EqualTo("2015-03-05_03.28.02"));
            Assert.That(after_second.Title, Is.EqualTo("2015-03-05_02.46.55"));

            Assert.That(new Guid[] { after_first.ID, after_second.ID }, Is.EqualTo(new Guid[] { second_guid, first_guid }));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _libManager.Dispose();

            var mwvm = Container.Resolve<IMainWindowViewModel>();
            mwvm.Close();

            GC.Collect();

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
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "PageOrderingTest");
        }
    }
}
