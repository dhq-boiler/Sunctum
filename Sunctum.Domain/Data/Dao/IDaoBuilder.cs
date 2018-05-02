

using Sunctum.Infrastructure.Data.Rdbms;

namespace Sunctum.Domain.Data.Dao
{
    public interface IDaoBuilder
    {
        IConnection CurrentConnection { get; set; }

        T Build<T>() where T : IDao;
    }
}
