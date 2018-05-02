

using System.Data.Common;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    public interface IConnection
    {
        string ConnectionString { get; }

        DbConnection OpenConnection();

        bool TableExists(string tableName);
    }
}
