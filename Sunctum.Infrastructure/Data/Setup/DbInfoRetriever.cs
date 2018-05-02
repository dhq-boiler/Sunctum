

using Sunctum.Infrastructure.Data.Rdbms;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Setup
{
    internal class DbInfoRetriever
    {
        public static IEnumerable<string> GetTableNames(IConnection connection)
        {
            using (var conn = connection.OpenConnection())
            {
                return conn.GetTableNames();
            }
        }
    }
}
