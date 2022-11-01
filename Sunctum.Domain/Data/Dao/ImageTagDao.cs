

using Homura.Extensions;
using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using Homura.QueryBuilder.Vendor.SQLite.Dml;
using NLog;
using Sunctum.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Sunctum.Domain.Data.Dao
{
    internal class ImageTagDao : SQLiteBaseDao<ImageTag>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public ImageTagDao() : base()
        { }

        public ImageTagDao(Type entityVersionType) : base(entityVersionType)
        { }

        public IEnumerable<ImageTag> FindByTagId(Guid tagId)
        {
            using (var conn = GetConnection())
            using (var command = conn.CreateCommand())
            {
                using (var query = new Select().Column("ImageID")
                                               .Column("TagID")
                                               .Column("Name")
                                               .From.Table(new Table<ImageTag>().Name)
                                               .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID")
                                               .Where.Column("TagID").EqualTo.Value(tagId))
                {
                    command.CommandText = query.ToSql();
                    query.SetParameters(command);
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

        public IEnumerable<ImageTag> FindByBookId(Guid bookId, DbConnection conn = null)
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
                    using (var query = new Select().Column("it", "ImageID")
                                                   .Column("it", "TagID")
                                                   .Column("t", "Name")
                                                   .From.Table(new Table<ImageTag>().Name, "it")
                                                   .Inner.Join(new Table<Tag>().Name, "t").On.Column("it", "TagID").EqualTo.Column("t", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("it", "ImageID").EqualTo.Column("i", "ID")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("i", "ID").EqualTo.Column("p", "ImageID")
                                                   .Inner.Join(new Table<Book>().Name, "b").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Where.Column("b", "ID").EqualTo.Value(bookId))
                    {
                        command.CommandText = query.ToSql();
                        query.SetParameters(command);
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

        public IEnumerable<ImageTag> FindByBookId(string anotherDatabaseAliasName, Guid bookId, DbConnection conn = null)
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
                    using (var query = new Select().Column("it", "ImageID")
                                                   .Column("it", "TagID")
                                                   .Column("t", "Name")
                                                   .From.Table(new Table<ImageTag>().Name, "it")
                                                   .Inner.Join(new Table<Tag>() { Schema = anotherDatabaseAliasName }.Name, "t").On.Column("it", "TagID").EqualTo.Column("t", "ID")
                                                   .Inner.Join(new Table<Image>() { Schema = anotherDatabaseAliasName }.Name, "i").On.Column("it", "ImageID").EqualTo.Column("i", "ID")
                                                   .Inner.Join(new Table<Page>() { Schema = anotherDatabaseAliasName }.Name, "p").On.Column("i", "ID").EqualTo.Column("p", "ImageID")
                                                   .Inner.Join(new Table<Book>() { Schema = anotherDatabaseAliasName }.Name, "b").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Where.Column("b", "ID").EqualTo.Value(bookId))
                    {
                        command.CommandText = query.ToSql();
                        query.SetParameters(command);
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

        public IEnumerable<ImageTag> FindBy(string anotherDatabaseAliasName, Dictionary<string, object> idDic, DbConnection conn = null)
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
                    using (var query = new Select().Column("ImageID")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>() { Schema = anotherDatabaseAliasName }.Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID")
                                                   .Where.KeyEqualToValue(idDic))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        query.SetParameters(command);

                        s_logger.Debug($"{sql} {query.GetParameters().ToStringKeyIsValue()}");
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

        public IEnumerable<ImageTag> FindBy(Dictionary<string, object> idDic, DbConnection conn = null)
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
                    using (var query = new Select().Column("ImageID")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>().Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID")
                                                   .Where.KeyEqualToValue(idDic))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        query.SetParameters(command);

                        s_logger.Debug($"{sql} {query.GetParameters().ToStringKeyIsValue()}");
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

        public IEnumerable<ImageTag> FindAll(string anotherDatabaseAliasName, DbConnection conn = null)
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
                    using (var query = new Select().Column("ImageID")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>() { Schema = anotherDatabaseAliasName }.Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID"))
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

        public IEnumerable<ImageTag> FindAll(DbConnection conn = null)
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
                    using (var query = new Select().Column("ImageID")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>().Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID"))
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

        public async IAsyncEnumerable<ImageTag> FindAllAsync(DbConnection conn = null)
        {
            bool isTransaction = conn != null;

            try
            {
                if (!isTransaction)
                {
                    conn = await GetConnectionAsync();
                }

                using (var command = conn.CreateCommand())
                {
                    using (var query = new Select().Column("ImageID")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>().Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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

        public async IAsyncEnumerable<TagCount> FindAllAsTagCountAsync(DbConnection conn = null)
        {
            bool isTransaction = conn != null;

            try
            {
                if (!isTransaction)
                {
                    conn = await GetConnectionAsync();
                }

                using (var command = conn.CreateCommand())
                {
                    using (var query = new Select().Count("*").As("Count")
                                                   .Column("TagID")
                                                   .Column("Name")
                                                   .From.Table(new Table<ImageTag>().Name)
                                                   .Inner.Join(new Table<Tag>().Name).On.Column("TagID").EqualTo.Column("ID")
                                                   .GroupBy.Column("TagID"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Guid tagId = reader.SafeGetGuid("TagID", Table);
                                string name = reader.SafeGetString("Name", Table);
                                int count = reader.SafeGetInt("Count", null);

                                var tag = new Tag();
                                tag.ID = tagId;
                                tag.Name = name;
                                yield return new TagCount(tag, count);
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

        public void DeleteByTagName(string tagName, DbConnection conn = null)
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
                    using (var query = new Delete().From.Table(new Table<ImageTag>())
                                                   .Where.Column("TagID").In.SubQuery(new Select().Column("ID")
                                                                                                  .From.Table(new Table<Tag>())
                                                                                                  .Where.Column("Name").EqualTo.Value(tagName)))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        query.SetParameters(command);

                        s_logger.Debug(sql);
                        int count = command.ExecuteNonQuery();
                        s_logger.Debug($"{count}件が処理されました．");
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

        public void BatchInsert(Tag tag, IEnumerable<Image> images, DbConnection conn = null)
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
                    using (var query = new InsertOrReplace().Into.Table(new Table<ImageTag>().Name)
                                                            .Columns("ImageID", "TagID")
                                                            .Values.Rows(images.Where(x => x is not null).Select(i => new object[] { i.ID, tag.ID })))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        query.SetParameters(command);

                        s_logger.Debug(sql);
                        int count = command.ExecuteNonQuery();
                        s_logger.Debug($"Processed {count} records.");
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

        protected override ImageTag ToEntity(IDataRecord reader)
        {
            Guid imageId = reader.SafeGetGuid("ImageID", Table);
            Guid tagId = reader.SafeGetGuid("TagID", Table);
            string name = reader.SafeGetString("Name", Table);

            var tag = new Tag();
            tag.ID = tagId;
            tag.Name = name;

            return new ImageTag()
            {
                ImageID = imageId,
                TagID = tagId,
            };
        }
    }
}
