using simpleqb.Iso.Dml.Syntaxes;
using System.Collections.Generic;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IUpdateColumnTransition : IColumnTransition<ISetClauseSyntax>
    {
        IValueExpressionSyntax KeyEqualToValue(Dictionary<string, object> columnValues);
    }
}
