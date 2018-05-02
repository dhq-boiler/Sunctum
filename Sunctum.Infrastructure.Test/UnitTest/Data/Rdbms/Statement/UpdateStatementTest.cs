

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
    public class UpdateStatementTest
    {
        [Test]
        public void UpdateTest()
        {
            IQueryBuilder query = new Update(new DummyPageTable())
                .UpdateSet(new ColumnNameBindValuePair("Title", "Test1"), new ColumnNameBindValuePair("BookID", Guid.Empty));

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE Page SET Title = @title_1, BookID = @bookid_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("Test1"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("title_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void UpdateIncrementingValueTest()
        {
            IQueryBuilder query = new Update(new DummyPageTable())
                .UpdateSet(new ColumnNameDirectValuePair("PageIndex", "PageIndex + 1"))
                .AddWhere("BookID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE Page SET PageIndex = PageIndex + 1 WHERE BookID = @bookid_1"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void UpdateWhereTest()
        {
            IQueryBuilder query = new Update(new DummyPageTable())
                .UpdateSet(new ColumnNameDirectValuePair("PageIndex", "99999"))
                .AddWhere("BookID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE Page SET PageIndex = 99999 WHERE BookID = @bookid_1"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void UpdateWhereInTest()
        {
            IQueryBuilder query = new Update(new DummyPageTable())
                .UpdateSet(new ColumnNameDirectValuePair("PageIndex", "99999"))
                .AddWhere(new In("BookID", new object[] { "1st", "2nd", "3rd" }));

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE Page SET PageIndex = 99999 WHERE BookID IN (@bookid_1, @bookid_2, @bookid_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("bookid_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("bookid_3"));
        }
    }
}
