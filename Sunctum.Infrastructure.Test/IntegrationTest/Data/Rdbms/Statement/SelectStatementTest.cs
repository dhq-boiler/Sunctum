

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
    public class SelectStatementTest
    {
        [Test]
        public void SelectTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere("Id", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE Id = @id_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectWhereInTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere(new In("Item1", new object[] { "1st", "2nd", "3rd" }));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE Item1 IN (@item1_1, @item1_2, @item1_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item1_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item1_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("item1_3"));
        }

        [Test]
        public void SelectWhereExistsTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere(new Exists(new Select(new Table<SomeEntity>())
                    .SelectColumn(Select.WILDCARD)
                    .AddWhere("Id", Guid.Empty)
                    .AsSubquery()));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE EXISTS (SELECT * FROM SomeEntity WHERE Id = @id_1)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectWhereNotExistsTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere(new NotExists(new Select(new Table<SomeEntity>())
                    .SelectColumn(Select.WILDCARD)
                    .AddWhere("Id", Guid.Empty)
                    .AsSubquery()));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE NOT EXISTS (SELECT * FROM SomeEntity WHERE Id = @id_1)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectOrderTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddOrderBy("Item1");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity ORDER BY Item1"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectLimitTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity LIMIT 1"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereOrderTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere("Id", Guid.Empty)
                .AddOrderBy("Item1");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE Id = @id_1 ORDER BY Item1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectOrderLimitTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddOrderBy("Item1")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity ORDER BY Item1 LIMIT 1"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereLimitTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere("Id", Guid.Empty)
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE Id = @id_1 LIMIT 1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectWhereOrderLimitTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddWhere("Id", Guid.Empty)
                .AddOrderBy("Item1")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity WHERE Id = @id_1 ORDER BY Item1 LIMIT 1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectInnerJoinTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddJoinOn(new InnerJoin(), new Table<AnotherEntity>(), new JoinOn("Item2", "Item1"))
                .AddWhere("Item2", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity INNER JOIN AnotherEntity ON Item2 = AnotherEntity.Item1 WHERE Item2 = @item2_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void SelectLeftJoinTest()
        {
            IQueryBuilder query = new Select(new Table<SomeEntity>())
                .SelectColumn("Id", "Item1", "Item2")
                .AddJoinOn(new LeftJoin(), new Table<AnotherEntity>(), new JoinOn("Item2", "Item1"))
                .AddWhere("Item2", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT Id, Item1, Item2 FROM SomeEntity LEFT JOIN AnotherEntity ON Item2 = AnotherEntity.Item1 WHERE Item2 = @item2_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item2_1"));
        }
    }
}
