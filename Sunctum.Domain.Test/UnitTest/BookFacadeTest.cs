

using Homura.ORM;
using NUnit.Framework;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using Sunctum.Domain.ViewModels;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class BookFacadeTest : TestSession
    {
        private static ILibrary s_libManager;

        private string _filePath;
        private readonly BookViewModel[] _books =
        {
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), "ブックタイトル１"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2),  "ブックタイトル２"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3),  "ブックタイトル３"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4),  "ブックタイトル４"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5),  "ブックタイトル５"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6),  "ブックタイトル６"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7),  "ブックタイトル７"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8),  "ブックタイトル８"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9),  "ブックタイトル９"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10), "ブックタイトル１０"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11), "ブックタイトル１１"),
            new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12), "ブックタイトル１２"),
        };

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "BookFacadeTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            s_libManager = Container.Resolve<ILibrary>();
            s_libManager.Initialize().Wait();
            s_libManager.Load().Wait();
        }

        [Test, Order(0)]
        public void SelectEmptyTest()
        {
            var items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(0));
        }

        [Test, Order(1)]
        public void CreateBooksTest()
        {
            foreach (var book in _books)
            {
                BookFacade.Insert(book);
            }

            var items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(12));
            Assert.That(items.ElementAt(0), Is.EqualTo(_books[0]));
            Assert.That(items.ElementAt(1), Is.EqualTo(_books[1]));
            Assert.That(items.ElementAt(2), Is.EqualTo(_books[2]));
            Assert.That(items.ElementAt(3), Is.EqualTo(_books[3]));
            Assert.That(items.ElementAt(4), Is.EqualTo(_books[4]));
            Assert.That(items.ElementAt(5), Is.EqualTo(_books[5]));
            Assert.That(items.ElementAt(6), Is.EqualTo(_books[6]));
            Assert.That(items.ElementAt(7), Is.EqualTo(_books[7]));
            Assert.That(items.ElementAt(8), Is.EqualTo(_books[8]));
            Assert.That(items.ElementAt(9), Is.EqualTo(_books[9]));
            Assert.That(items.ElementAt(10), Is.EqualTo(_books[10]));
            Assert.That(items.ElementAt(11), Is.EqualTo(_books[11]));
        }

        [Test, Order(2)]
        public void UpdateBooksTest()
        {
            var changeList = new BookViewModel[]
            {
                new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), "ブックタイトル１＋"),
                new BookViewModel(new System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12), "ブックタイトル１２＋"),
            };

            BookFacade.Update(changeList[0]);
            BookFacade.Update(changeList[1]);

            var items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(12));
            Assert.That(items.ElementAt(0), Is.EqualTo(changeList[0]));
            Assert.That(items.ElementAt(1), Is.EqualTo(_books[1]));
            Assert.That(items.ElementAt(2), Is.EqualTo(_books[2]));
            Assert.That(items.ElementAt(3), Is.EqualTo(_books[3]));
            Assert.That(items.ElementAt(4), Is.EqualTo(_books[4]));
            Assert.That(items.ElementAt(5), Is.EqualTo(_books[5]));
            Assert.That(items.ElementAt(6), Is.EqualTo(_books[6]));
            Assert.That(items.ElementAt(7), Is.EqualTo(_books[7]));
            Assert.That(items.ElementAt(8), Is.EqualTo(_books[8]));
            Assert.That(items.ElementAt(9), Is.EqualTo(_books[9]));
            Assert.That(items.ElementAt(10), Is.EqualTo(_books[10]));
            Assert.That(items.ElementAt(11), Is.EqualTo(changeList[1]));
        }

        [Test, Order(3)]
        public void DeleteBooksTest()
        {
            var items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(12));

            BookFacade.DeleteWhereIDIs(_books[0].ID);
            BookFacade.DeleteWhereIDIs(_books[1].ID);
            BookFacade.DeleteWhereIDIs(_books[2].ID);
            BookFacade.DeleteWhereIDIs(_books[3].ID);
            BookFacade.DeleteWhereIDIs(_books[4].ID);
            BookFacade.DeleteWhereIDIs(_books[5].ID);

            items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(6));

            BookFacade.DeleteWhereIDIs(_books[6].ID);
            BookFacade.DeleteWhereIDIs(_books[7].ID);
            BookFacade.DeleteWhereIDIs(_books[8].ID);
            BookFacade.DeleteWhereIDIs(_books[9].ID);
            BookFacade.DeleteWhereIDIs(_books[10].ID);
            BookFacade.DeleteWhereIDIs(_books[11].ID);

            items = BookFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(0));
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
