

using Homura.ORM;
using NUnit.Framework;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Test.Core;
using Sunctum.Domain.ViewModels;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class AuthorFacadeTest : TestSession
    {
        private static ILibrary s_libManager;
        private string _filePath;

        private readonly AuthorViewModel[] _authors =
        {
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), "難波 英彦"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2), "大嶽 政宏"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3), "笛木 邦弘"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4), "井下 尚彦"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5), "出井 謙司"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6), "與那覇 幹生"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7), "瀧上 泰寛"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8), "笠野 卓志"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9), "今冨 ひろき"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10), "北潟 マサヒロ"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1), "Dennis G King"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2), "Rodney G. Matthews"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3), "Prashant N Shrestha"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 4), "Bo H Pearson"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 5), "Herman Patrick Ashley"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 6), "Kieran Henry Durham"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 7), "Gage Hassan Lancaster"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 8), "Salvador Dela Cruz Bruno"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 9), "Jagdish Jack Elder"),
            new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 10), "Gurpreet Sanchez Sorensen"),
        };

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "AuthorFacadeTest.db");
            ConnectionManager.SetDefaultConnection($"Data Source={_filePath}", typeof(SQLiteConnection));

            s_libManager = Container.Resolve<ILibrary>();
            await s_libManager.Initialize().ConfigureAwait(false);
            await s_libManager.Load().ConfigureAwait(false);
        }

        [Test, Order(0)]
        public void SelectEmptyTest()
        {
            var items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(0));
        }

        [Test, Order(1)]
        public void CreateAuthorsTest()
        {
            foreach (var author in _authors)
            {
                AuthorFacade.Create(author);
            }

            var items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(20));
            Assert.That(items.ElementAt(0), Is.EqualTo(_authors[0]));
            Assert.That(items.ElementAt(1), Is.EqualTo(_authors[1]));
            Assert.That(items.ElementAt(2), Is.EqualTo(_authors[2]));
            Assert.That(items.ElementAt(3), Is.EqualTo(_authors[3]));
            Assert.That(items.ElementAt(4), Is.EqualTo(_authors[4]));
            Assert.That(items.ElementAt(5), Is.EqualTo(_authors[5]));
            Assert.That(items.ElementAt(6), Is.EqualTo(_authors[6]));
            Assert.That(items.ElementAt(7), Is.EqualTo(_authors[7]));
            Assert.That(items.ElementAt(8), Is.EqualTo(_authors[8]));
            Assert.That(items.ElementAt(9), Is.EqualTo(_authors[9]));
            Assert.That(items.ElementAt(10), Is.EqualTo(_authors[10]));
            Assert.That(items.ElementAt(11), Is.EqualTo(_authors[11]));
            Assert.That(items.ElementAt(12), Is.EqualTo(_authors[12]));
            Assert.That(items.ElementAt(13), Is.EqualTo(_authors[13]));
            Assert.That(items.ElementAt(14), Is.EqualTo(_authors[14]));
            Assert.That(items.ElementAt(15), Is.EqualTo(_authors[15]));
            Assert.That(items.ElementAt(16), Is.EqualTo(_authors[16]));
            Assert.That(items.ElementAt(17), Is.EqualTo(_authors[17]));
            Assert.That(items.ElementAt(18), Is.EqualTo(_authors[18]));
            Assert.That(items.ElementAt(19), Is.EqualTo(_authors[19]));
        }

        [Test, Order(2)]
        public void UpdateAuthorsTest()
        {
            var changeList = new AuthorViewModel[]
            {
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), "村杉 周治"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2), "角屋 譲司"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3), "松嵜 紀昭"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4), "司馬 悠"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5), "今藤 勝一"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1), "Duy Owen Lovell"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2), "Cool Hong Rajesh"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3), "Rafi Nath Post"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 4), "Raven Jude Heard"),
                new AuthorViewModel(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 5), "Jeric Kelvin Yeager")
            };

            AuthorFacade.Update(changeList[0]);
            AuthorFacade.Update(changeList[1]);
            AuthorFacade.Update(changeList[2]);
            AuthorFacade.Update(changeList[3]);
            AuthorFacade.Update(changeList[4]);
            AuthorFacade.Update(changeList[5]);
            AuthorFacade.Update(changeList[6]);
            AuthorFacade.Update(changeList[7]);
            AuthorFacade.Update(changeList[8]);
            AuthorFacade.Update(changeList[9]);

            var items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(20));
            Assert.That(items.ElementAt(0), Is.EqualTo(changeList[0]));
            Assert.That(items.ElementAt(1), Is.EqualTo(changeList[1]));
            Assert.That(items.ElementAt(2), Is.EqualTo(changeList[2]));
            Assert.That(items.ElementAt(3), Is.EqualTo(changeList[3]));
            Assert.That(items.ElementAt(4), Is.EqualTo(changeList[4]));
            Assert.That(items.ElementAt(5), Is.EqualTo(_authors[5]));
            Assert.That(items.ElementAt(6), Is.EqualTo(_authors[6]));
            Assert.That(items.ElementAt(7), Is.EqualTo(_authors[7]));
            Assert.That(items.ElementAt(8), Is.EqualTo(_authors[8]));
            Assert.That(items.ElementAt(9), Is.EqualTo(_authors[9]));
            Assert.That(items.ElementAt(10), Is.EqualTo(changeList[5]));
            Assert.That(items.ElementAt(11), Is.EqualTo(changeList[6]));
            Assert.That(items.ElementAt(12), Is.EqualTo(changeList[7]));
            Assert.That(items.ElementAt(13), Is.EqualTo(changeList[8]));
            Assert.That(items.ElementAt(14), Is.EqualTo(changeList[9]));
            Assert.That(items.ElementAt(15), Is.EqualTo(_authors[15]));
            Assert.That(items.ElementAt(16), Is.EqualTo(_authors[16]));
            Assert.That(items.ElementAt(17), Is.EqualTo(_authors[17]));
            Assert.That(items.ElementAt(18), Is.EqualTo(_authors[18]));
            Assert.That(items.ElementAt(19), Is.EqualTo(_authors[19]));
        }

        [Test, Order(3)]
        public void DeleteAuthorsTest()
        {
            var items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(20));

            AuthorFacade.Delete(_authors[0].ID);
            AuthorFacade.Delete(_authors[1].ID);
            AuthorFacade.Delete(_authors[2].ID);
            AuthorFacade.Delete(_authors[3].ID);
            AuthorFacade.Delete(_authors[4].ID);

            items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(15));

            AuthorFacade.Delete(_authors[15].ID);
            AuthorFacade.Delete(_authors[16].ID);
            AuthorFacade.Delete(_authors[17].ID);
            AuthorFacade.Delete(_authors[18].ID);
            AuthorFacade.Delete(_authors[19].ID);

            items = AuthorFacade.FindAll();
            Assert.That(items.Count, Is.EqualTo(10));
        }

        [OneTimeTearDown]
        public void _OneTimeTearDown()
        {
            s_libManager.Dispose();

            ConnectionManager.DisposeAllDebris();

            GC.Collect();

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
