
using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using Homura.QueryBuilder.Vendor.SQLite.Dcl;
using Homura.QueryBuilder.Vendor.SQLite.Dml;
using NLog;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao
{
    public abstract class SQLiteBaseDao<E> : Dao<E> where E : EntityBaseObject
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public SQLiteBaseDao()
            : base()
        { }

        public SQLiteBaseDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        public static void Vacuum(IConnection connection)
        {
            s_logger.Info($"SQLite VACUUM Operation will start as soon.");

            using (var conn = connection.OpenConnection())
            {
                if (conn.GetType() == typeof(SQLiteConnection))
                {
                    using (var command = conn.CreateCommand())
                    {
                        using (var query = new Vacuum())
                        {
                            string sql = query.ToSql();
                            command.CommandText = sql;
                            command.CommandType = System.Data.CommandType.Text;

                            s_logger.Debug(sql);
                            command.ExecuteNonQuery();
                        }
                        s_logger.Info($"SQLite VACUUM Operation finnished.");
                    }
                }
            }
        }

        public void InsertOrReplace(E entity, DbConnection conn = null)
        {
            InitializeColumnDefinitions();
            try
            {
                VerifyColumnDefinitions(conn);
            }
            catch (NotMatchColumnException e)
            {
                throw new DatabaseSchemaException($"Didn't insert because mismatch definition of table:{TableName}", e);
            }

            QueryHelper.KeepTryingUntilProcessSucceed(() =>
            {
                QueryHelper.ForDao.ConnectionInternal(this, new Action<DbConnection>((connection) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        var overrideColumns = SwapIfOverrided(Columns);

                        using (var query = new InsertOrReplace().Into.Table(new Table<E>().Name)
                                                                .Columns(overrideColumns.Select(c => c.ColumnName))
                                                                .Values.Row(overrideColumns.Select(c => c.PropInfo.GetValue(entity))))
                        {
                            string sql = query.ToSql();
                            command.CommandText = sql;
                            query.SetParameters(command);

                            s_logger.Debug($"{sql} {query.GetParameters().ToStringKeyIsValue()}");
                            int inserted = command.ExecuteNonQuery();
                            if (inserted == 0)
                            {
                                throw new NoEntityInsertedException($"Failed:{sql} {query.GetParameters().ToStringKeyIsValue()}");
                            }
                        }
                    }
                }), conn);
            });
        }

        public async Task InsertOrReplaceAsync(E entity, DbConnection conn = null)
        {
            InitializeColumnDefinitions();
            try
            {
                VerifyColumnDefinitions(conn);
            }
            catch (NotMatchColumnException e)
            {
                throw new DatabaseSchemaException($"Didn't insert because mismatch definition of table:{TableName}", e);
            }

            await QueryHelper.KeepTryingUntilProcessSucceedAsync<Task>(async () =>
            {
                await QueryHelper.ForDao.ConnectionInternalAsync(this, new Action<DbConnection>(async (connection) =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        var overrideColumns = SwapIfOverrided(Columns);

                        using (var query = new InsertOrReplace().Into.Table(new Table<E>().Name)
                                                                .Columns(overrideColumns.Select(c => c.ColumnName))
                                                                .Values.Row(overrideColumns.Select(c => c.PropInfo.GetValue(entity))))
                        {
                            string sql = query.ToSql();
                            command.CommandText = sql;
                            query.SetParameters(command);

                            s_logger.Debug($"{sql} {query.GetParameters().ToStringKeyIsValue()}");
                            int inserted = await command.ExecuteNonQueryAsync();
                            if (inserted == 0)
                            {
                                throw new NoEntityInsertedException($"Failed:{sql} {query.GetParameters().ToStringKeyIsValue()}");
                            }
                        }
                    }
                }), conn);
            });
        }
    }
}
