
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class NotIn : In
    {
        public NotIn(string leftColumnName, IRightValue rightValue)
            : base(leftColumnName, rightValue)
        { }

        public NotIn(string columnName, object[] parameter)
            : base(columnName, parameter)
        { }

        public NotIn(string columnName, Select subquery)
            : base(columnName, subquery)
        { }

        public override string KEYWORD { get; } = "NOT IN";
    }
}
