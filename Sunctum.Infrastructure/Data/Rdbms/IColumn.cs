

using System.Collections.Generic;
using System.Reflection;
using Sunctum.Infrastructure.Data.Rdbms.Ddl;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    public interface IColumn
    {
        string ColumnName { get; }

        string DataType { get; }

        IEnumerable<IDdlConstraint> Constraints { get; }

        int Order { get; }

        PropertyInfo PropInfo { get; }

        string ConstraintsToSql();

        PlaceholderRightValue ToParameter(EntityBaseObject entity);

        PlaceholderRightValue ToParameter(Dictionary<string, object> idDic);
    }
}
