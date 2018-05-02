using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IInsertColumnSyntax : ISyntaxBase, IColumnTransition<IInsertColumnSyntax>, IValuesTransition
    {
        ISelectSyntax Select { get; }
    }
}
