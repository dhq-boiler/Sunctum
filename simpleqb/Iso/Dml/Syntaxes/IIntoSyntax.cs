using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IIntoSyntax : ISyntaxBase, ITableTransition<IInsertColumnSyntax>
    {
    }
}
