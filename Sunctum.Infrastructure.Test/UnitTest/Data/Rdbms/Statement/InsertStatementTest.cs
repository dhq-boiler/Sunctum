

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping;
using Sunctum.Infrastructure.Data.Rdbms.Dml;
using Sunctum.Infrastructure.Test.TestFixture;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using System;
using System.Linq;

namespace Sunctum.Infrastructure.Test.UnitTest.Data.Rdbms.Statement
{
    [Category("Infrastructure")]
    [Category("UnitTest")]
    [TestFixture]
    public class InsertStatementTest
    {
        [Test]
        public void InsertTest()
        {
            IQueryBuilder query = new Insert(new DummyPageTable())
                .InsertValue("vID", "vTitle", "vBookID", "vImageID", "vPageIndex");

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO Page VALUES(@id_1, @title_1, @bookid_1, @imageid_1, @pageindex_1)"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("vID"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo("vTitle"));
            Assert.That(query.Parameters.ElementAt(2).Values.First(), Is.EqualTo("vBookID"));
            Assert.That(query.Parameters.ElementAt(3).Values.First(), Is.EqualTo("vImageID"));
            Assert.That(query.Parameters.ElementAt(4).Values.First(), Is.EqualTo("vPageIndex"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("title_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("bookid_1"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("imageid_1"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("pageindex_1"));
        }

        [Test]
        public void InsertDefineTest()
        {
            IQueryBuilder query = new Insert(new DummyPageTable())
                .InsertColumns("ID", "Title", "BookID", "ImageID", "PageIndex")
                .InsertValue("vID", "vTitle", "vBookID", "vImageID", "vPageIndex");

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO Page (ID, Title, BookID, ImageID, PageIndex) VALUES(@id_1, @title_1, @bookid_1, @imageid_1, @pageindex_1)"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("vID"));
            Assert.That(query.Parameters.ElementAt(1).Values.First(), Is.EqualTo("vTitle"));
            Assert.That(query.Parameters.ElementAt(2).Values.First(), Is.EqualTo("vBookID"));
            Assert.That(query.Parameters.ElementAt(3).Values.First(), Is.EqualTo("vImageID"));
            Assert.That(query.Parameters.ElementAt(4).Values.First(), Is.EqualTo("vPageIndex"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("title_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("bookid_1"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("imageid_1"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("pageindex_1"));
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

            IQueryBuilder query = new Insert(new DummyImageTagTable())
                .InsertColumns("ImageID", "TagID");

            query.InsertValue(id_1, id_2);
            query.InsertValue(id_3, id_4);
            query.InsertValue(id_5, id_6);

            Assert.That(query.ToSql(), Is.EqualTo("INSERT INTO ImageTag (ImageID, TagID) VALUES(@imageid_1, @tagid_1), (@imageid_2, @tagid_2), (@imageid_3, @tagid_3)"));
            Assert.That(query.Parameters.ElementAt(0).Values.ElementAt(0), Is.EqualTo(id_1));
            Assert.That(query.Parameters.ElementAt(1).Values.ElementAt(0), Is.EqualTo(id_2));
            Assert.That(query.Parameters.ElementAt(2).Values.ElementAt(0), Is.EqualTo(id_3));
            Assert.That(query.Parameters.ElementAt(3).Values.ElementAt(0), Is.EqualTo(id_4));
            Assert.That(query.Parameters.ElementAt(4).Values.ElementAt(0), Is.EqualTo(id_5));
            Assert.That(query.Parameters.ElementAt(5).Values.ElementAt(0), Is.EqualTo(id_6));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("imageid_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("tagid_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("imageid_2"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("tagid_2"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("imageid_3"));
            Assert.That(query.Parameters.ElementAt(5).Name, Is.EqualTo("tagid_3"));
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
                new Table<Header>(typeof(VersionOrigin)),
                new Table<Header>(typeof(Version_1)),
                "INSERT INTO Header_1 (Id, Item1, Item2) SELECT Id, Item1, Item2 FROM Header"
            },
            new object[] {
                new Table<Header>(typeof(Version_1)),
                new Table<Header>(typeof(VersionOrigin)),
                "INSERT INTO Header (Id, Item1, Item2) SELECT Id, Item1, Item2 FROM Header_1"
            },
        };
    }
}
