namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOuterJoinTypeSyntax : IJoinTypeSyntax
    {
        IJoinTypeSyntax Outer { get; }
    }
}
