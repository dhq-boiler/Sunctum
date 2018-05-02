
using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface ICloseSyntax<R> : ISyntaxBase, IWhereTransition<R>, INaturalTransition, IJoinTypeTransition, IOuterJoinTypeTransition, ICrossJoinTransition, IJoinTableTransition<IJoinTableSyntax>, IOrderByTransition, IGroupByTransition, ISql where R : class
    { }
}
