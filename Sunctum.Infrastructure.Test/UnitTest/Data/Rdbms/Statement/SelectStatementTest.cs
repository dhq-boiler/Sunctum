

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
    public class SelectStatementTest
    {
        [Test]
        public void SelectTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere("BookID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE BookID = @bookid_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void SelectWhereInTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere(new In("Title", new object[] { "1st", "2nd", "3rd" }));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE Title IN (@title_1, @title_2, @title_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("title_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("title_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("title_3"));
        }

        [Test]
        public void SelectWhereExistsTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere(new Exists(new Select(new DummyPageTable())
                    .SelectColumn(Select.WILDCARD)
                    .AddWhere("ID", Guid.Empty)
                    .AsSubquery()));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE EXISTS (SELECT * FROM Page WHERE ID = @id_1)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectWhereNotExistsTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere(new NotExists(new Select(new DummyPageTable())
                    .SelectColumn(Select.WILDCARD)
                    .AddWhere("ID", Guid.Empty)
                    .AsSubquery()));

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE NOT EXISTS (SELECT * FROM Page WHERE ID = @id_1)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
        }

        [Test]
        public void SelectOrderTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddOrderBy("PageIndex");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page ORDER BY PageIndex"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectLimitTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page LIMIT 1"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereOrderTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere("BookID", Guid.Empty)
                .AddOrderBy("PageIndex");

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE BookID = @bookid_1 ORDER BY PageIndex"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void SelectOrderLimitTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddOrderBy("PageIndex")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page ORDER BY PageIndex LIMIT 1"));
            Assert.That(query.Parameters.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SelectWhereLimitTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere("BookID", Guid.Empty)
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE BookID = @bookid_1 LIMIT 1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void SelectWhereOrderLimitTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ID", "Title", "BookID", "ImageID", "PageIndex")
                .AddWhere("BookID", Guid.Empty)
                .AddOrderBy("PageIndex")
                .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ID, Title, BookID, ImageID, PageIndex FROM Page WHERE BookID = @bookid_1 ORDER BY PageIndex LIMIT 1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bookid_1"));
        }

        [Test]
        public void SelectInnerJoinTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ImageID", "TagID", "Name")
                .AddJoinOn(new InnerJoin(), new DummyTagTable(), new JoinOn("TagID", "ID"))
                .AddWhere("TagID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ImageID, TagID, Name FROM Page INNER JOIN Tag ON TagID = Tag.ID WHERE TagID = @tagid_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("tagid_1"));
        }

        [Test]
        public void SelectLeftJoinTest()
        {
            IQueryBuilder query = new Select(new DummyPageTable())
                .SelectColumn("ImageID", "TagID", "Name")
                .AddJoinOn(new LeftJoin(), new DummyTagTable(), new JoinOn("TagID", "ID"))
                .AddWhere("TagID", Guid.Empty);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT ImageID, TagID, Name FROM Page LEFT JOIN Tag ON TagID = Tag.ID WHERE TagID = @tagid_1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("tagid_1"));
        }

        [Test]
        public void SelectMultiJoinTest()
        {
            var query = new Select(new DummyBookTable("b"))
                        .SelectColumn(
                            "b.ID as bId",
                            "b.AuthorID as bAuthorId",
                            "a.ID as aId",
                            "a.Name as aName",
                            "p.ID as pId",
                            "p.Title as pTitle",
                            "p.ImageId as pImageId",
                            "p.PageIndex as pIndex",
                            "i.ID as iId",
                            "i.Title as iTitle",
                            "i.MasterPath as iMasterPath",
                            "t.ID as tId",
                            "t.ImageID as tImageId",
                            "t.Path as tPath"
                            )
                        .AddJoinOn(new LeftJoin(), new DummyAuthorTable("a"), new JoinOn("a.ID", "bAuthorId", false))
                        .AddJoinOn(new InnerJoin(), new DummyPageTable("p"), new JoinOn("p.BookID", "b.ID", false))
                        .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("i.ID", "pImageId", false))
                        .AddJoinOn(new LeftJoin(), new DummyThumbnailTable("t"), new JoinOn("i.ID", "tImageId", false))
                        .AddWhere("bId", Guid.Empty)
                        .AddOrderBy("pIndex")
                        .LimitBy(1);

            Assert.That(query.ToSql(), Is.EqualTo("SELECT b.ID as bId, b.AuthorID as bAuthorId, a.ID as aId, a.Name as aName, p.ID as pId, p.Title as pTitle, p.ImageId as pImageId, p.PageIndex as pIndex, i.ID as iId, i.Title as iTitle, i.MasterPath as iMasterPath, t.ID as tId, t.ImageID as tImageId, t.Path as tPath FROM Book b LEFT JOIN Author a ON a.ID = bAuthorId INNER JOIN Page p ON p.BookID = b.ID INNER JOIN Image i ON i.ID = pImageId LEFT JOIN Thumbnail t ON i.ID = tImageId WHERE bId = @bid_1 ORDER BY pIndex LIMIT 1"));
            Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("bid_1"));
        }
    }
}
