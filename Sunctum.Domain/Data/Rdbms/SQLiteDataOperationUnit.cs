

using System.Collections.Generic;
using System.Data;
using Homura.ORM;
using NLog;

namespace Sunctum.Domain.Data.Rdbms
{
    public class SQLiteDataOperationUnit : DataOperationUnit
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private HashSet<string> _attachedDatabaseAliasNames;

        public SQLiteDataOperationUnit() : base()
        {
            _attachedDatabaseAliasNames = new HashSet<string>();
        }

        public void AttachDatabase(string anotherDatabaseFile, string anotherDatabaseAliasName)
        {
            using (var command = CurrentConnection.CreateCommand())
            {
                string sql = $"attach database \"{anotherDatabaseFile}\" as {anotherDatabaseAliasName}";
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                s_logger.Debug(sql);
                command.ExecuteNonQuery();

                _attachedDatabaseAliasNames.Add(anotherDatabaseAliasName);
            }
        }

        public void DetachDatabase(string anotherDatabaseAliasName)
        {
            using (var command = CurrentConnection.CreateCommand())
            {
                string sql = $"detach database {anotherDatabaseAliasName}";
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                s_logger.Debug(sql);
                command.ExecuteNonQuery();

                _attachedDatabaseAliasNames.Remove(anotherDatabaseAliasName);
            }
        }

        public void DetachAllDatabases()
        {
            foreach (var alias in new List<string>(_attachedDatabaseAliasNames))
            {
                DetachDatabase(alias);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (CurrentConnection != null)
                    {
                        DetachAllDatabases();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
