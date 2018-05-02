using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IGroupByColumnSyntax : ISyntaxBase, IColumnTransition<IGroupByColumnSyntax>, IWhereTransition<IConditionValueSyntax>, IOrderByTransition, INaturalTransition, IJoinTypeTransition, IOuterJoinTypeTransition, ISql
    {
    }
}
