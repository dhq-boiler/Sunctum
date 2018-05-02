

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
    public class UnionStatementTest
    {
        [Test]
        public void UnionTest()
        {
            var bookIds = new Guid[] { Guid.Empty };
            var pageIds = new Guid[] { Guid.Empty, Guid.Empty };
            var imageIds = new Guid[] { Guid.Empty, Guid.Empty, Guid.Empty };

            var query = new Union(
                            new Select(new DummyBookTable("b"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyPageTable("p"), new JoinOn("p.BookID", "b.ID", false))
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("b.ID", bookIds.Cast<object>().ToArray())),
                            new Select(new DummyPageTable("p"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("p.ID", pageIds.Cast<object>().ToArray())),
                            new Select(new DummyImageTable("i"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddWhere(new In("ID", imageIds.Cast<object>().ToArray()))
                        );

            Assert.That(query.ToSql(), Is.EqualTo("SELECT i.ID, i.Title, i.MasterPath FROM Book b INNER JOIN Page p ON p.BookID = b.ID INNER JOIN Image i ON p.ImageID = i.ID WHERE b.ID IN (@b_id_1) UNION SELECT i.ID, i.Title, i.MasterPath FROM Page p INNER JOIN Image i ON p.ImageID = i.ID WHERE p.ID IN (@p_id_1, @p_id_2) UNION SELECT i.ID, i.Title, i.MasterPath FROM Image i WHERE ID IN (@id_1, @id_2, @id_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("b_id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("p_id_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("p_id_2"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("id_2"));
            Assert.That(query.Parameters.ElementAt(5).Name, Is.EqualTo("id_3"));
        }


        [Test]
        public void UnionAbbr1stTest()
        {
            var bookIds = new Guid[] { };
            var pageIds = new Guid[] { Guid.Empty, Guid.Empty };
            var imageIds = new Guid[] { Guid.Empty, Guid.Empty, Guid.Empty };

            var query = new Union(
                            new Select(new DummyBookTable("b"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyPageTable("p"), new JoinOn("p.BookID", "b.ID", false))
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("b.ID", bookIds.Cast<object>().ToArray())),
                            new Select(new DummyPageTable("p"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("p.ID", pageIds.Cast<object>().ToArray())),
                            new Select(new DummyImageTable("i"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddWhere(new In("ID", imageIds.Cast<object>().ToArray()))
                        );

            Assert.That(query.ToSql(), Is.EqualTo("SELECT i.ID, i.Title, i.MasterPath FROM Book b INNER JOIN Page p ON p.BookID = b.ID INNER JOIN Image i ON p.ImageID = i.ID WHERE b.ID IN () UNION SELECT i.ID, i.Title, i.MasterPath FROM Page p INNER JOIN Image i ON p.ImageID = i.ID WHERE p.ID IN (@p_id_1, @p_id_2) UNION SELECT i.ID, i.Title, i.MasterPath FROM Image i WHERE ID IN (@id_1, @id_2, @id_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("p_id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("p_id_2"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("id_2"));
            Assert.That(query.Parameters.ElementAt(4).Name, Is.EqualTo("id_3"));
        }


        [Test]
        public void UnionAbbr2ndTest()
        {
            var bookIds = new Guid[] { Guid.Empty };
            var pageIds = new Guid[] { };
            var imageIds = new Guid[] { Guid.Empty, Guid.Empty, Guid.Empty };

            var query = new Union(
                            new Select(new DummyBookTable("b"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyPageTable("p"), new JoinOn("p.BookID", "b.ID", false))
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("b.ID", bookIds.Cast<object>().ToArray())),
                            new Select(new DummyPageTable("p"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("p.ID", pageIds.Cast<object>().ToArray())),
                            new Select(new DummyImageTable("i"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddWhere(new In("ID", imageIds.Cast<object>().ToArray()))
                        );

            Assert.That(query.ToSql(), Is.EqualTo("SELECT i.ID, i.Title, i.MasterPath FROM Book b INNER JOIN Page p ON p.BookID = b.ID INNER JOIN Image i ON p.ImageID = i.ID WHERE b.ID IN (@b_id_1) UNION SELECT i.ID, i.Title, i.MasterPath FROM Page p INNER JOIN Image i ON p.ImageID = i.ID WHERE p.ID IN () UNION SELECT i.ID, i.Title, i.MasterPath FROM Image i WHERE ID IN (@id_1, @id_2, @id_3)"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("b_id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("id_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("id_2"));
            Assert.That(query.Parameters.ElementAt(3).Name, Is.EqualTo("id_3"));
        }

        [Test]
        public void UnionAbbr3rdTest()
        {
            var bookIds = new Guid[] { Guid.Empty };
            var pageIds = new Guid[] { Guid.Empty, Guid.Empty };
            var imageIds = new Guid[] { };

            var query = new Union(
                            new Select(new DummyBookTable("b"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyPageTable("p"), new JoinOn("p.BookID", "b.ID", false))
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("b.ID", bookIds.Cast<object>().ToArray())),
                            new Select(new DummyPageTable("p"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddJoinOn(new InnerJoin(), new DummyImageTable("i"), new JoinOn("p.ImageID", "i.ID", false))
                             .AddWhere(new In("p.ID", pageIds.Cast<object>().ToArray())),
                            new Select(new DummyImageTable("i"))
                             .SelectColumn(
                                 "i.ID",
                                 "i.Title",
                                 "i.MasterPath"
                             )
                             .AddWhere(new In("ID", imageIds.Cast<object>().ToArray()))
                        );

            Assert.That(query.ToSql(), Is.EqualTo("SELECT i.ID, i.Title, i.MasterPath FROM Book b INNER JOIN Page p ON p.BookID = b.ID INNER JOIN Image i ON p.ImageID = i.ID WHERE b.ID IN (@b_id_1) UNION SELECT i.ID, i.Title, i.MasterPath FROM Page p INNER JOIN Image i ON p.ImageID = i.ID WHERE p.ID IN (@p_id_1, @p_id_2) UNION SELECT i.ID, i.Title, i.MasterPath FROM Image i WHERE ID IN ()"));
            Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("b_id_1"));
            Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("p_id_1"));
            Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("p_id_2"));
        }
    }
}
