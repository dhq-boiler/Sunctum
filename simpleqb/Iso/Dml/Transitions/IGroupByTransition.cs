using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IGroupByTransition
    {
        IGroupBySyntax GroupBy { get; }
    }
}
