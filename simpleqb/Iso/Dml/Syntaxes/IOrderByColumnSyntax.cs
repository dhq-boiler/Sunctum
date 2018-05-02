
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOrderByColumnSyntax : IOrderBySyntax, IWhereTransition<IConditionValueSyntax>, INaturalTransition, IJoinTypeTransition, IOuterJoinTypeTransition, ISql
    {
        IOrderTypeSyntax Asc { get; }

        IOrderTypeSyntax Desc { get; }
    }
}
