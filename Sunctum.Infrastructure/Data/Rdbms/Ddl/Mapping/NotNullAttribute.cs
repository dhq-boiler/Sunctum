

using System;

namespace Sunctum.Infrastructure.Data.Rdbms.Ddl.Mapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute, IDdlConstraintAttribute
    {
        public NotNullAttribute()
        { }

        public IDdlConstraint ToConstraint()
        {
            return new NotNull();
        }
    }
}
