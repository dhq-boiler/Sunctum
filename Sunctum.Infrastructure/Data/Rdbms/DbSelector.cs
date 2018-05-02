﻿

using System;
using System.Data.Common;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    internal class DbSelector
    {
        public Type DbConnectionType { get; private set; }

        public DbSelector(Type dbConnectionType)
        {
            DbConnectionType = dbConnectionType;
        }

        public DbConnection CreateConnection()
        {
            return CreateConnection(DbConnectionType);
        }

        public static DbConnection CreateConnection(Type type)
        {
            return (DbConnection)Activator.CreateInstance(type);
        }
    }
}
