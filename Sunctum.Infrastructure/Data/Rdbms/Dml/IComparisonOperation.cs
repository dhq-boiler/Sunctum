
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface IComparisonOperation : ISqlize
    {
        string LeftColumnName { get; set; }

        IRightValue RightValue { get; set; }
    }
}
