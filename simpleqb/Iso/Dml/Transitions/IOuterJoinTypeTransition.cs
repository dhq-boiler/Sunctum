using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IOuterJoinTypeTransition
    {
        IOuterJoinTypeSyntax Left { get; }

        IOuterJoinTypeSyntax Right { get; }

        IOuterJoinTypeSyntax Full { get; }
    }
}
