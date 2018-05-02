using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IOrderByTransition
    {
        IOrderBySyntax OrderBy { get; }
    }
}
