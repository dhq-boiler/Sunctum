

using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IConditionValueSyntax : ISyntaxBase, IOrderByTransition, INaturalTransition, IJoinTypeTransition, IOuterJoinTypeTransition, IGroupByTransition, ISql
    { }
}
