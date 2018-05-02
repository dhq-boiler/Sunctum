using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOrderTypeSyntax : IOrderBySyntax, IWhereTransition<IConditionValueSyntax>, INaturalTransition, IJoinTypeTransition, IOuterJoinTypeTransition
    {
    }
}
