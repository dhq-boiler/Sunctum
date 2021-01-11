

using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using NLog;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sunctum.Domain.Data.Dao
{
    internal class TagDao : SQLiteBaseDao<Tag>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public TagDao() : base()
        { }

        public TagDao(Type entityVersionType) : base(entityVersionType)
        { }

        public IEnumerable<Tag> FindAll(string anotherDatabaseAliasName, DbConnection conn = null)
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
                    using (var query = new Select().Asterisk()
                                                   .From.Table(new Table<Tag>() { Schema = anotherDatabaseAliasName }))
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

        protected override Tag ToEntity(IDataRecord reader)
        {
            return new Tag()
            {
                ID = reader.SafeGetGuid("ID"),
                Name = reader.SafeGetString("Name"),
            };
        }
    }
}
