using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IJoinTypeTransition
    {
        IJoinTypeSyntax Inner { get; }

        IUnionSyntax Union { get; }
    }
}
