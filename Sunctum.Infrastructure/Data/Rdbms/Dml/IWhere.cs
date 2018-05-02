
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface IWhere : ISqlize
    {
        List<IRightValue> Parameters { get; }

        void Add(LogicalOperator and_or, string columnName, object parameter);

        void Add(LogicalOperator and_or, string columnName, object[] parameters);

        void Add(LogicalOperator and_or, string columnName, Select subquery);

        void Add(LogicalOperator and_or, IIsNull is_null);

        void Add(LogicalOperator and_or, IIn inoperator);

        void Add(LogicalOperator and_or, IExists exists);
    }
}
