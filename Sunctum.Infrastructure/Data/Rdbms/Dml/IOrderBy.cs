
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface IOrderBy : ISqlize
    {
        void Add(string columnName, Ordering ordering);
    }
}
