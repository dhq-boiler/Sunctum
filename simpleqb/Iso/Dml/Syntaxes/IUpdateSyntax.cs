
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IUpdateSyntax : ISyntaxBase, ITableTransition<IUpdateTableSyntax>
    {
    }
}
