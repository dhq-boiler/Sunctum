

using simpleqb.Iso.Dml;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    public class IntermediateTableDao : SQLiteBaseDao<Dummy>
    {
        public IEnumerable<BookImageChain> FindAll(DbConnection conn = null)
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
                    using (var query = new Select().Column("b", "ID").As("bId")
                                                   .Column("i", "ID").As("iId")
                                                   .From.Table(new Table<Book>().Name, "b")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("i", "ID").EqualTo.Column("p", "ImageID"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                yield return new BookImageChain()
                                {
                                    BookId = reader.SafeGetGuid("bId"),
                                    ImageId = reader.SafeGetGuid("iId")
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
