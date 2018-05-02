using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface ICorrespondingSyntax : IQueryTermTransition
    {
        IBySyntax By { get; }
    }
}
