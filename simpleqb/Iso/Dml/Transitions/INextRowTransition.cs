using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface INextRowTransition
    {
        IValuesSyntax NextRow { get; }
    }
}
