

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Dml;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using System;
using System.Linq;

namespace Sunctum.Infrastructure.Test.IntegrationTest.Data.Rdbms.Statement
{
    [Category("Infrastructure")]
    [Category("IntegrationTest")]
    [TestFixture]
    public class InsertStatementTest
    {
        [Test]
        public void InsertTest()
        {
            IQueryBuilder query = new Insert(new Table<SomeEntity>())
                .InsertValue("vId", "vItem1", "vItem2");

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO SomeEntity VALUES(@id_1, @item1_1, @item2_1)"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("vId"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo("vItem1"));
            Assert.That(query.Parameters.ElementAt(2).Values.First(), Is.EqualTo("vItem2"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item1_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void InsertDefineTest()
        {
            IQueryBuilder query = new Insert(new Table<SomeEntity>())
                .InsertColumns("Id", "Item1", "Item2")
                .InsertValue("vId", "vItem1", "vItem2");

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO SomeEntity (Id, Item1, Item2) VALUES(@id_1, @item1_1, @item2_1)"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("vId"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo("vItem1"));
            Assert.That(query.Parameters.ElementAt(2).Values.First(), Is.EqualTo("vItem2"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item1_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("item2_1"));
        }

        [Test]
        public void InsertMultipleRecordsTest()
        {
            var id_1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            var id_2 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
            var id_3 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);
            var id_4 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4);
            var id_5 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5);
            var id_6 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6);

            IQueryBuilder query = new Insert(new Table<SomeEntity>())
                .InsertColumns("Id", "Item1");

            query.InsertValue(id_1, "vItem1_1");
            query.InsertValue(id_2, "vItem1_2");
            query.InsertValue(id_3, "vItem1_3");
            query.InsertValue(id_4, "vItem1_4");
            query.InsertValue(id_5, "vItem1_5");
            query.InsertValue(id_6, "vItem1_6");

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO SomeEntity (Id, Item1) VALUES(@id_1, @item1_1), (@id_2, @item1_2), (@id_3, @item1_3), (@id_4, @item1_4), (@id_5, @item1_5), (@id_6, @item1_6)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("item1_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("id_2"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("item1_2"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("id_3"));
            Assert.That(query.Parameters.ElementAt(5).Name, Is.EqualTo("item1_3"));
            Assert.That(query.Parameters.ElementAt(6).Name, Is.EqualTo("id_4"));
            Assert.That(query.Parameters.ElementAt(7).Name, Is.EqualTo("item1_4"));
            Assert.That(query.Parameters.ElementAt(8).Name, Is.EqualTo("id_5"));
            Assert.That(query.Parameters.ElementAt(9).Name, Is.EqualTo("item1_5"));
            Assert.That(query.Parameters.ElementAt(10).Name, Is.EqualTo("id_6"));
            Assert.That(query.Parameters.ElementAt(11).Name, Is.EqualTo("item1_6"));
            Assert.That(query.Parameters.ElementAt(0).Values.ElementAt(0), Is.EqualTo(id_1));
            Assert.That(query.Parameters.ElementAt(1).Values.ElementAt(0), Is.EqualTo("vItem1_1"));
            Assert.That(query.Parameters.ElementAt(2).Values.ElementAt(0), Is.EqualTo(id_2));
            Assert.That(query.Parameters.ElementAt(3).Values.ElementAt(0), Is.EqualTo("vItem1_2"));
            Assert.That(query.Parameters.ElementAt(4).Values.ElementAt(0), Is.EqualTo(id_3));
            Assert.That(query.Parameters.ElementAt(5).Values.ElementAt(0), Is.EqualTo("vItem1_3"));
            Assert.That(query.Parameters.ElementAt(6).Values.ElementAt(0), Is.EqualTo(id_4));
            Assert.That(query.Parameters.ElementAt(7).Values.ElementAt(0), Is.EqualTo("vItem1_4"));
            Assert.That(query.Parameters.ElementAt(8).Values.ElementAt(0), Is.EqualTo(id_5));
            Assert.That(query.Parameters.ElementAt(9).Values.ElementAt(0), Is.EqualTo("vItem1_5"));
            Assert.That(query.Parameters.ElementAt(10).Values.ElementAt(0), Is.EqualTo(id_6));
            Assert.That(query.Parameters.ElementAt(11).Values.ElementAt(0), Is.EqualTo("vItem1_6"));
        }

        [Test]
        [TestCaseSource("s_testCaseSource_InsertSelectTest")]
        public void InsertSelectTest<X, Y>(Table<X> from, Table<Y> to, string expected) where X : EntityBaseObject
                                                                                        where Y : EntityBaseObject
        {
            var newTable = to;
            var oldTable = from;
            var commonColumns = oldTable.Columns.Intersect(newTable.Columns).Select(c => c.ColumnName);
            var query = new Insert(newTable)
                .InsertColumns(commonColumns.ToArray())
                .InsertFromSubquery(new Select(oldTable).SelectColumn(commonColumns.ToArray()).AsSubquery());

            Assert.That(query.ToSql(), Is.EqualTo(expected));
        }

        private static object[] s_testCaseSource_InsertSelectTest =
        {
            new object[] {
                new Table<SomeEntity>(typeof(VersionOrigin)),
                new Table<SomeEntity>(typeof(Version_1)),
                "INSERT INTO SomeEntity_1 (Id, Item1, Item2) SELECT Id, Item1, Item2 FROM SomeEntity"
            },
            new object[] {
                new Table<SomeEntity>(typeof(Version_1)),
                new Table<SomeEntity>(typeof(VersionOrigin)),
                "INSERT INTO SomeEntity (Id, Item1, Item2) SELECT Id, Item1, Item2 FROM SomeEntity_1"
            },
        };
    }
}
