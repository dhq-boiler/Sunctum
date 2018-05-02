

using NLog;
using simpleqb.Iso.Dml;
using Sunctum.Domain.Data.Entity;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    internal class AuthorDao : SQLiteBaseDao<Author>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public AuthorDao() : base()
        { }

        public AuthorDao(Type entityVersionType) : base(entityVersionType)
        { }

        public IEnumerable<Author> FindAll(string anotherDatabaseAliasName, DbConnection conn = null)
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
                    using (var query = new Select().Asterisk().From.Table(new Table<Author>() { Schema = anotherDatabaseAliasName }))
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

        public IEnumerable<AuthorCount> FindAllAsCountOrderByNameAsc(DbConnection conn = null)
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
                    using (var query = new Select().Count("*").As("Count").Columns("ID", "Name")
                                                    .From.Table(new Table<Author>())
                                                    .Inner.Join(new Table<Book>().Name).On.Column("ID").EqualTo.Column("AuthorID")
                                                    .GroupBy.Column("ID")
                                                    .OrderBy.Column("Name"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid authorId = reader.SafeGetGuid("ID");
                                string name = reader.SafeGetString("Name");
                                int count = reader.SafeGetInt("Count");

                                yield return new AuthorCount(new Author(authorId, name), count);
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

        public IEnumerable<AuthorCount> FindAllAsCountOrderByNameDesc(DbConnection conn = null)
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
                    using (var query = new Select().Count("*").Columns("ID", "Name")
                                                   .From.Table(new Table<Author>().Name)
                                                   .Inner.Join(new Table<Book>().Name).On.Column("ID").EqualTo.Column("AuthorID")
                                                   .GroupBy.Column("ID")
                                                   .OrderBy.Column("Name").Desc)
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid authorId = reader.SafeGetGuid("ID");
                                string name = reader.SafeGetString("Name");
                                int count = reader.SafeGetInt("Count");

                                yield return new AuthorCount(new Author(authorId, name), count);
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

        public IEnumerable<AuthorCount> FindAllAsCountOrderByCountAsc(DbConnection conn = null)
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
                    using (var query = new Select().Count("*").As("Count").Columns("ID", "Name")
                                                   .From.Table(new Table<Author>().Name)
                                                   .Inner.Join(new Table<Book>().Name).On.Column("ID").EqualTo.Column("AuthorID")
                                                   .GroupBy.Column("ID")
                                                   .OrderBy.Column("Count"))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid authorId = reader.SafeGetGuid("ID");
                                string name = reader.SafeGetString("Name");
                                int count = reader.SafeGetInt("Count");

                                yield return new AuthorCount(new Author(authorId, name), count);
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

        public IEnumerable<AuthorCount> FindAllAsCountOrderByCountDesc(DbConnection conn = null)
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
                    using (var query = new Select().Count("*").As("Count").Column("a", "ID").Column("a", "Name")
                                                   .From.Table(new Table<Author>().Name, "a")
                                                   .Inner.Join(new Table<Book>().Name, "b").On.Column("a", "ID").EqualTo.Column("b", "AuthorID")
                                                   .GroupBy.Column("a", "ID")
                                                   .OrderBy.Column("Count").Desc)
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;

                        s_logger.Debug(sql);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid authorId = reader.SafeGetGuid("ID");
                                string name = reader.SafeGetString("Name");
                                int count = reader.SafeGetInt("Count");

                                yield return new AuthorCount(new Author(authorId, name), count);
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

        protected override Author ToEntity(IDataRecord reader)
        {
            return new Author()
            {
                ID = reader.SafeGetGuid("ID"),
                Name = reader.SafeGetString("Name"),
            };
        }
    }
}
