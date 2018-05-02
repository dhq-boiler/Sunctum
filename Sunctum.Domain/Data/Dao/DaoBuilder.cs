

using Sunctum.Infrastructure.Data.Rdbms;
using System;

namespace Sunctum.Domain.Data.Dao
{
    public class DaoBuilder : IDaoBuilder
    {
        public IConnection CurrentConnection { get; set; }

        public DaoBuilder()
        { }

        public DaoBuilder(IConnection connection)
        {
            CurrentConnection = connection;
        }

        public T Build<T>() where T : IDao
        {
            var dao = Activator.CreateInstance<T>();
            dao.CurrentConnection = CurrentConnection;
            return dao;
        }
    }
}
