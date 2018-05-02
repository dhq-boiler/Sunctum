using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IQueryTermTransition : ICrossJoinTransition, IJoinTypeTransition, IOuterJoinTypeTransition, INaturalTransition
    {
        ISelectSyntax Select { get; }
    }
}
