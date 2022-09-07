

using Homura.Extensions;
using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.QueryBuilder.Iso.Dml;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    public class IntermediateTableDao : SQLiteBaseDao<Dummy>
    {
        public IEnumerable<BookTag> FindAll(DbConnection conn = null)
        {
            bool isTransaction = conn != null;

            try
            {
                if (!isTransaction)
                {
                    conn = GetConnection();
                }

                using (var command = conn.CreateCommand())
                {
                    using (var query = new Select().Distinct
                                                   .Column("b", "ID").As("bId")
                                                   .Column("it", "TagID").As("itTagId")
                                                   .From.Table(new Table<Book>(typeof(VersionOrigin)).Name, "b")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("i", "ID").EqualTo.Column("p", "ImageID")
                                                   .Inner.Join(new Table<ImageTag>().Name, "it").On.Column("it", "ImageID").EqualTo.Column("i", "ID"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                yield return new BookTag()
                                {
                                    BookID = reader.SafeGetGuid("bId", null),
                                    TagID = reader.SafeGetGuid("itTagId", null)
                                };
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!isTransaction)
                {
                    conn.Dispose();
                }
            }
        }

        protected override Dummy ToEntity(IDataRecord reader)
        {
            throw new NotSupportedException();
        }
    }
}
