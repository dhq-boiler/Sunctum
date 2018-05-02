using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IAsSyntax : ISyntaxBase, IColumnTransition<IColumnSyntax>, IFromTransition<IFromSyntax<ICloseSyntax<IConditionValueSyntax>>>
    {
    }
}
