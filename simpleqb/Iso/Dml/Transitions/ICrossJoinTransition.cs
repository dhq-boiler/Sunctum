using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface ICrossJoinTransition
    {
        ICrossSyntax Cross { get; }
    }
}
