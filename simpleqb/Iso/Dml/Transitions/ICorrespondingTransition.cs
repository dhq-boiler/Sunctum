using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface ICorrespondingTransition : IQueryTermTransition
    {
        ICorrespondingSyntax Corresponding { get; }
    }
}
