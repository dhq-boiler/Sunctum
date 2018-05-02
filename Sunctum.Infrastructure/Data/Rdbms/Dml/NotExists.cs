
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class NotExists : Exists
    {
        public NotExists(Select subquery)
            : base(subquery)
        { }

        public override string KEYWORD { get; } = "NOT EXISTS";
    }
}
