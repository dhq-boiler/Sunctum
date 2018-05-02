
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IExistsSyntax<R> : ISyntaxBase, ISubQueryTransition<R> where R : class
    {
    }
}
