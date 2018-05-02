

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Dml;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using System;
using System.Linq;

namespace Sunctum.Infrastructure.Test.IntegrationTest.Data.Rdbms.Statement
{
    [Category("Infrastructure")]
    [Category("IntegrationTest")]
    [TestFixture]
    public class UpdateStatementTest
    {
        [Test]
        public void UpdateTest()
        {
            IQueryBuilder query = new Update(new Table<SomeEntity>())
                .UpdateSet(new ColumnNameBindValuePair("Item1", "Test1"), new ColumnNameBindValuePair("Item2", Guid.Empty));

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE SomeEntity SET Item1 = @item1_1, Item2 = @item2_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("Test1"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item1_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void UpdateIncrementingValueTest()
        {
            IQueryBuilder query = new Update(new Table<SomeEntity>())
                .UpdateSet(new ColumnNameDirectValuePair("Item1", "Item1 + 1"))
                .AddWhere("Item2", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE SomeEntity SET Item1 = Item1 + 1 WHERE Item2 = @item2_1"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void UpdateWhereTest()
        {
            IQueryBuilder query = new Update(new Table<SomeEntity>())
                .UpdateSet(new ColumnNameDirectValuePair("Item1", "99999"))
                .AddWhere("Item2", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE SomeEntity SET Item1 = 99999 WHERE Item2 = @item2_1"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void UpdateWhereInTest()
        {
            IQueryBuilder query = new Update(new Table<SomeEntity>())
                .UpdateSet(new ColumnNameDirectValuePair("Item1", "99999"))
                .AddWhere(new In("Item2", new object[] { "1st", "2nd", "3rd" }));

            Assert.That(query.ToSql(), Is.EqualTo("UPDATE SomeEntity SET Item1 = 99999 WHERE Item2 IN (@item2_1, @item2_2, @item2_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item2_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item2_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("item2_3"));
        }
    }
}
