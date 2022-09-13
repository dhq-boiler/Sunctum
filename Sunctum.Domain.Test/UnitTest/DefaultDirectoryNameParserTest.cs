

using System.Linq;
using NLog;
using NUnit.Framework;
using Sunctum.Domain.Logic.Parse;

namespace Sunctum.Domain.Test.UnitTest
{
    [Category("UnitTest")]
    [TestFixture]
    public class DefaultDirectoryNameParserTest
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        [Test]
        public void タイトルのみ()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("タイトル");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 空著者タイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("[]タイトル");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 著者タイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("[著者]タイトル");

            Assert.That(dnp.Author, Is.EqualTo("著者"));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 空タグタイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("()タイトル");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void タグタイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("(タグ)タイトル");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(1));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ"));
        }

        [Test]
        public void 複タグタイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("(タグ１,タグ２)タイトル");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(2));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ１"));
            Assert.That(dnp.Tags.ElementAt(1), Is.EqualTo("タグ２"));
        }

        [Test]
        public void タグ著者タイトル()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse("(タグ)[著者]タイトル");

            Assert.That(dnp.Author, Is.EqualTo("著者"));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(1));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ"));
        }

        [Test]
        public void タイトルのみスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" タイトル ");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 空著者タイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" [ ] タイトル ");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 著者タイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" [ 著者 ] タイトル ");

            Assert.That(dnp.Author, Is.EqualTo("著者"));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void 空タグタイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" ( ) タイトル ");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(0));
        }

        [Test]
        public void タグタイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" ( タグ ) タイトル ");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(1));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ"));
        }

        [Test]
        public void 複タグタイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" ( タグ１ , タグ２ ) タイトル ");

            Assert.That(dnp.Author, Is.EqualTo(null));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(2));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ１"));
            Assert.That(dnp.Tags.ElementAt(1), Is.EqualTo("タグ２"));
        }

        [Test]
        public void タグ著者タイトルスペース混入()
        {
            DefaultDirectoryNameParser dnp = new DefaultDirectoryNameParser();
            dnp.Parse(" ( タグ ) [ 著者 ] タイトル ");

            Assert.That(dnp.Author, Is.EqualTo("著者"));
            Assert.That(dnp.Title, Is.EqualTo("タイトル"));
            Assert.That(dnp.Tags.Count(), Is.EqualTo(1));
            Assert.That(dnp.Tags.ElementAt(0), Is.EqualTo("タグ"));
        }
    }
}
