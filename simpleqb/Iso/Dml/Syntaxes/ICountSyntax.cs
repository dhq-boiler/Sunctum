using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface ICountSyntax : ISyntaxBase, IColumnTransition<IColumnSyntax>, IFromTransition<IFromSyntax<ICloseSyntax<IConditionValueSyntax>>>, IAsTransition
    { }
}
