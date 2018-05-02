
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IFromSyntax<Return> : ISyntaxBase, ITableTransition<Return>, ISubQueryTransition<Return> where Return : class
    {
    }
}
