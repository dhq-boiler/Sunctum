

using NUnit.Framework;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Dml;
using Sunctum.Infrastructure.Test.TestFixture.Entity;
using Sunctum.Infrastructure.Test.TestFixture.Migration;
using System;
using System.Linq;

namespace Sunctum.Infrastructure.Test.IntegrationTest.Data.Rdbms.Statement
{
    public class DeleteStatementTest
    {
        [Category("Infrastructure")]
        [Category("IntegrationTest")]
        [TestFixture]
        public class WithTable_SchemaVersionManager_Version_Origin
        {
            [Test]
            public void DeleteTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>());

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity"));
                Assert.That(query.Parameters.Count(), Is.EqualTo(0));
            }

            [Test]
            public void DeleteWhereTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>())
                    .AddWhere("Id", Guid.Empty);

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity WHERE Id = @id_1"));
                Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            }

            [Test]
            public void DeleteWhereInTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>())
                    .AddWhere(new In("Id", new object[] { "1st", "2nd", "3rd" }));

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity WHERE Id IN (@id_1, @id_2, @id_3)"));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
                Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("id_2"));
                Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("id_3"));
            }

            [Test]
            public void DeleteSubqueryInTest()
            {
                var query = new Delete(new Table<SomeEntity>())
                            .AddWhere(new In("Id", new Select(new Table<AnotherEntity>())
                                                    .SelectColumn("Id")
                                                    .AddWhere("Item1", "Value1")
                                                    .AsSubquery()));

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity WHERE Id IN (SELECT Id FROM AnotherEntity WHERE Item1 = @item1_1)"));
                Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("Value1"));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item1_1"));
            }
        }

        [Category("Infrastructure")]
        [Category("IntegrationTest")]
        [TestFixture]
        public class WithTable_SchemaVersionManager_Version_1
        {
            [Test]
            public void DeleteTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>(typeof(Version_1)));

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity_1"));
                Assert.That(query.Parameters.Count(), Is.EqualTo(0));
            }

            [Test]
            public void DeleteWhereTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>(typeof(Version_1)))
                    .AddWhere("Id", Guid.Empty);

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity_1 WHERE Id = @id_1"));
                Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo(Guid.Empty));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
            }

            [Test]
            public void DeleteWhereInTest()
            {
                IQueryBuilder query = new Delete(new Table<SomeEntity>(typeof(Version_1)))
                    .AddWhere(new In("Id", new object[] { "1st", "2nd", "3rd" }));

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity_1 WHERE Id IN (@id_1, @id_2, @id_3)"));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("id_1"));
                Assert.That(query.Parameters.ElementAt(1).Name, Is.EqualTo("id_2"));
                Assert.That(query.Parameters.ElementAt(2).Name, Is.EqualTo("id_3"));
            }

            [Test]
            public void DeleteSubqueryInTest()
            {
                var query = new Delete(new Table<SomeEntity>(typeof(Version_1)))
                            .AddWhere(new In("Id", new Select(new Table<AnotherEntity>())
                                                    .SelectColumn("Id")
                                                    .AddWhere("Item1", "Value1")
                                                    .AsSubquery()));

                Assert.That(query.ToSql(), Is.EqualTo("DELETE FROM SomeEntity_1 WHERE Id IN (SELECT Id FROM AnotherEntity WHERE Item1 = @item1_1)"));
                Assert.That(query.Parameters.ElementAt(0).Values.First(), Is.EqualTo("Value1"));
                Assert.That(query.Parameters.ElementAt(0).Name, Is.EqualTo("item1_1"));
            }
        }
    }
}
