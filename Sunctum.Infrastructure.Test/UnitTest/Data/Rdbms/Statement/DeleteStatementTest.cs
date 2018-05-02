

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms.Dml;
using Sunctum.Infrastructure.Test.TestFixture;
using System;
using System.Linq;

namespace Sunctum.Infrastructure.Test.UnitTest.Data.Rdbms.Statement
{
    [Category("Infrastructure")]
    [Category("UnitTest")]
    [TestFixture]
    public class DeleteStatementTest
    {
        [Test]
        public void DeleteTest()
        {
            IQueryBuilder query = new Delete(new DummyPageTable());

            Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM Page"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteWhereTest()
        {
            IQueryBuilder query = new Delete(new DummyPageTable())
                .AddWhere("BookID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM Page WHERE BookID = @bookid_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void DeleteWhereInTest()
        {
            IQueryBuilder query = new Delete(new DummyPageTable())
                .AddWhere(new In("BookID", new object[] { "1st", "2nd", "3rd" }));

            Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM Page WHERE BookID IN (@bookid_1, @bookid_2, @bookid_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("bookid_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("bookid_3"));
        }

        [Test]
        public void DeleteSubqueryInTest()
        {
            var query = new Delete(new DummyImageTagTable())
                        .AddWhere(new In("TagID", new Select(new DummyTagTable())
                                                .SelectColumn("ID")
                                                .AddWhere("Name", "TagValue")
                                                .AsSubquery()));

            Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM ImageTag WHERE TagID IN (SELECT ID FROM Tag WHERE Name = @name_1)"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("TagValue"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("name_1"));
        }
    }
}
