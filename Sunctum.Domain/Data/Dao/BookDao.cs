

using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using Homura.QueryBuilder.Vendor.SQLite.Dml;
using NLog;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    internal class BookDao : SQLiteBaseDao<Book>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public BookDao() : base()
        { }

        public BookDao(Type entityVersionType) : base(entityVersionType)
        { }

        public IEnumerable<Book> FindAll(string anotherDatabaseAliasName, DbConnection conn = null)
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
                    using (var query = new Select().Asterisk().From.Table(new Table<Book>() { Schema = anotherDatabaseAliasName }))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                yield return ToEntity(reader);
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

        public IEnumerable<BookViewModel> FindAllWithAuthor(DbConnection conn = null)
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
                                                   .Left.Join(new Table<Star>().Name, "s").On.Column("s", "TypeId").EqualTo.Value(0).And().Column("s", "ID").EqualTo.Column("bId"))
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

        protected override Book ToEntity(IDataRecord reader)
        {
            return new Book()
            {
                ID = reader.SafeGetGuid("ID", Table),
                Title = reader.SafeGetString("Title", Table),
                AuthorID = reader.SafeGetGuid("AuthorID", Table),
                PublishDate = reader.SafeGetNullableDateTime("PublishDate", Table),
                ByteSize = reader.SafeNullableGetLong("ByteSize", Table),
                FingerPrint = reader.SafeGetString("FingerPrint", Table),
            };
        }

        internal void GetProperty(ref BookViewModel book, DbConnection conn = null)
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
                                                   .Column("b", "AuthorID").As("bAuthorId")
                                                   .Column("a", "ID").As("aId")
                                                   .Column("a", "Name").As("aName")
                                                   .Column("p", "ID").As("pId")
                                                   .Column("p", "Title").As("pTitle")
                                                   .Column("p", "ImageId").As("pImageId")
                                                   .Column("p", "PageIndex").As("pIndex")
                                                   .Column("i", "ID").As("iId")
                                                   .Column("i", "Title").As("iTitle")
                                                   .Column("i", "MasterPath").As("iMasterPath")
                                                   .Column("t", "ID").As("tId")
                                                   .Column("t", "ImageID").As("tImageId")
                                                   .Column("t", "Path").As("tPath")
                                                   .From.Table(new Table<Book>().Name, "b")
                                                   .Left.Join(new Table<Author>().Name, "a").On.Column("a", "ID").EqualTo.Column("bAuthorId")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("i", "ID").EqualTo.Column("pImageId")
                                                   .Left.Join(new Table<Thumbnail>().Name, "t").On.Column("i", "ID").EqualTo.Column("tImageId")
                                                   .Where.Column("bId").EqualTo.Value(book.ID)
                                                   .OrderBy.Column("pIndex")
                                                   .Limit(1))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        query.SetParameters(command);

                        using (var rdr = command.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                if (!rdr.IsDBNull("aId") && !rdr.IsDBNull("aName"))
                                {
                                    var author = new AuthorViewModel();
                                    author.ID = rdr.SafeGetGuid("aId", null);
                                    author.Name = rdr.SafeGetString("aName", null);
                                    book.Author = author;
                                }

                                var page = new PageViewModel();
                                page.Configuration = Configuration.ApplicationConfiguration;
                                page.ID = rdr.SafeGetGuid("pId", null);
                                page.Title = rdr.SafeGetString("pTitle", null);
                                page.BookID = rdr.SafeGetGuid("bId", null);
                                page.ImageID = rdr.SafeGetGuid("pImageId", null);
                                page.PageIndex = rdr.SafeGetInt("pIndex", null);
                                book.FirstPage = page;

                                var image = new ImageViewModel();
                                image.Configuration = Configuration.ApplicationConfiguration;
                                image.ID = rdr.SafeGetGuid("iId", null);
                                image.Title = rdr.SafeGetString("iTitle", null);
                                image.RelativeMasterPath = rdr.SafeGetString("iMasterPath", null);
                                page.Image = image;

                                if (!rdr.IsDBNull("tId") && !rdr.IsDBNull("tImageId") && !rdr.IsDBNull("tPath"))
                                {
                                    var thumbnail = new ThumbnailViewModel();
                                    thumbnail.ID = rdr.SafeGetGuid("tId", null);
                                    thumbnail.ImageID = rdr.SafeGetGuid("tImageId", null);
                                    thumbnail.RelativeMasterPath = rdr.SafeGetString("tPath", null);
                                    image.Thumbnail = thumbnail;
                                }
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
