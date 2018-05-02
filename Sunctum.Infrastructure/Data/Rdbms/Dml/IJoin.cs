
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface IJoin : ISqlize
    {
        ITable RightTable { get; set; }

        void AddJoinOn(params JoinOn[] joinOns);
    }
}
