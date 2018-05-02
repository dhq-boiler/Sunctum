
namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    public interface IDdlConstraintAttribute
    {
        IDdlConstraint ToConstraint();
    }
}
