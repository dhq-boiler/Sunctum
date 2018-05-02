
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class IsNotNull : IsNull
    {
        public override string KEYWORD { get; } = "IS NOT NULL";

        public IsNotNull(string leftColumnName)
            : base(leftColumnName)
        { }
    }
}
