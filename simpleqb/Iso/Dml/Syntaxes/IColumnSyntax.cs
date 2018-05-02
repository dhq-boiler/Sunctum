using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IColumnSyntax : ISyntaxBase, IColumnTransition<IColumnSyntax>, IAsteriskTransition<IColumnSyntax>, ISubQueryTransition<IColumnSyntax>, IFromTransition<IFromSyntax<ICloseSyntax<IConditionValueSyntax>>>, IAsTransition
    {
    }
}
