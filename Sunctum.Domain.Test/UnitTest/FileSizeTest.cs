

using NUnit.Framework;
using Sunctum.Domain.Util;

namespace Sunctum.Domain.Test.UnitTest
{
    [Ignore("テストが永遠に終わらない")]
    [Category("UnitTest")]
    [TestFixture]
    public class FileSizeTest
    {
        [Test]
        public void Bytes0()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(0), Is.EqualTo("0 bytes"));
        }

        [Test]
        public void Bytes1()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1), Is.EqualTo("1 bytes"));
        }

        [Test]
        public void Bytes511()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(511), Is.EqualTo("511 bytes"));
        }

        [Test]
        public void Bytes512()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(512), Is.EqualTo("512 bytes"));
        }

        [Test]
        public void Bytes1023()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1023), Is.EqualTo("1,023 bytes"));
        }

        [Test]
        public void Bytes1024()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1024), Is.EqualTo("1.0 KB"));
        }

        [Test]
        public void KBytes511()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(511 * 1024), Is.EqualTo("511 KB"));
        }

        [Test]
        public void KBytes512()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(512 * 1024), Is.EqualTo("512 KB"));
        }

        [Test]
        public void KBytes1023()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1023 * 1024), Is.EqualTo("1,023 KB"));
        }

        [Test]
        public void KBytes1024()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1024 * 1024), Is.EqualTo("1.0 MB"));
        }


        [Test]
        public void MBytes511()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(511 * 1024 * 1024), Is.EqualTo("511 MB"));
        }

        [Test]
        public void MBytes512()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(512 * 1024 * 1024), Is.EqualTo("512 MB"));
        }

        [Test]
        public void MBytes1023()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1023 * 1024 * 1024), Is.EqualTo("1,023 MB"));
        }

        [Test]
        public void MBytes1024()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1024 * 1024 * 1024), Is.EqualTo("1.0 GB"));
        }

        [Test]
        public void GBytes511()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(511L * 1024 * 1024 * 1024), Is.EqualTo("511 GB"));
        }

        [Test]
        public void GBytes512()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(512L * 1024 * 1024 * 1024), Is.EqualTo("512 GB"));
        }

        [Test]
        public void GBytes1023()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1023L * 1024 * 1024 * 1024), Is.EqualTo("1,023 GB"));
        }

        [Test]
        public void GBytes1024()
        {
            Assert.That(FileSize.ConvertFileSizeUnit(1024L * 1024 * 1024 * 1024), Is.EqualTo("1,024 GB"));
        }
    }
}
