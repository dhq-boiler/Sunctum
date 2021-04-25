

using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    internal class StarDao : SQLiteBaseDao<Star>
    {
        public StarDao()
            : base()
        { }

        public StarDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Star ToEntity(IDataRecord reader)
        {
            return new Star()
            {
                ID = reader.SafeGetGuid("ID", Table),
                TypeId = reader.SafeGetInt("TypeId", Table),
                Level = reader.SafeGetInt("Level", Table)
            };
        }

        internal IEnumerable<BookViewModel> FindBookByStar(int? level, DbConnection conn = null)
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
                    var dic = new Dictionary<string, object>() { { "s.Level", level } };
                    using (var query = new Select().Column("b", "ID").As("bId")
                                                   .Column("b", "Title").As("bTitle")
                                                   .Column("b", "AuthorID").As("bAuthorId")
                                                   .Column("b", "ByteSize").As("bByteSize")
                                                   .Column("b", "PublishDate").As("bPublishDate")
                                                   .Column("a", "ID").As("aId")
                                                   .Column("a", "Name").As("aName")
                                                   .Column("s", "Level").As("sLevel")
                                                   .Column("b", "FingerPrint").As("bFingerPrint")
                                                   .From.Table(new Table<Book>().Name, "b")
                                                   .Left.Join(new Table<Author>().Name, "a").On.Column("a", "ID").EqualTo.Column("bAuthorId")
                                                   .Left.Join(new Table<Star>().Name, "s").On.Column("s", "TypeId").EqualTo.Value(0)
                                                        .And().Column("s", "ID").EqualTo.Column("bId")
                                                   .Where.KeyEqualToValue(dic))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        query.SetParameters(command);

                        using (var rdr = command.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var book = new BookViewModel();
                                book.Configuration = Configuration.ApplicationConfiguration;
                                book.ID = rdr.SafeGetGuid("bId", null);
                                book.Title = rdr.SafeGetString("bTitle", null);
                                book.ByteSize = rdr.SafeNullableGetLong("bByteSize", null);
                                book.PublishDate = rdr.SafeGetNullableDateTime("bPublishDate", null);
                                if (!rdr.IsDBNull("aId") && !rdr.IsDBNull("aName"))
                                {
                                    var author = new AuthorViewModel();
                                    author.ID = rdr.SafeGetGuid("aId", null);
                                    author.Name = rdr.SafeGetString("aName", null);
                                    book.Author = author;
                                }
                                book.StarLevel = rdr.SafeGetNullableInt("sLevel", null);
                                book.FingerPrint = rdr.SafeGetString("bFingerPrint", null);
                                book.ContentsRegistered = true;

                                yield return book;
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
    }
}
