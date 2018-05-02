using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IWhereTransition<R> where R : class
    {
        IWhereSyntax<R, IOperatorSyntax<R>, IIsSyntax<R>> Where { get; }
    }
}
