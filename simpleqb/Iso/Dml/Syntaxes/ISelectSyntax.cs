using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface ISelectSyntax : ISyntaxBase, IQuantityTransition, IColumnTransition<IColumnSyntax>, IFunctionTransition<ICountSyntax>
    {
    }
}
