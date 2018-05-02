using NUnit.Framework;
using simpleqb.Vendor.SQLite.Dcl;

namespace simpleqb.Test.Vendor.SQLite.Dcl
{
    [TestFixture]
    [Category("simpleqb QueryBuilder")]
    public class VacuumTest
    {
        [Test]
        public void Vacuum()
        {
            using (var query = new Vacuum())
            {
                Assert.That(query.ToSql(), Is.EqualTo("VACUUM"));
            }
        }
    }
}
