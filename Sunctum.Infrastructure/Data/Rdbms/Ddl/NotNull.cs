
namespace Sunctum.Infrastructure.Data.Rdbms.Ddl
{
    public class NotNull : IDdlConstraint
    {
        public string ToSql()
        {
            return "NOT NULL";
        }
    }
}
