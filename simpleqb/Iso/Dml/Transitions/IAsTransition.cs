using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IAsTransition
    {
        IAsSyntax As(string columnAlias);
    }
}
