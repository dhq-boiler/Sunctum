
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOperatorSyntax<R> : ISyntaxBase, ISubQueryTransition<R> where R : class
    {
        R Value(object value);
    }
}
