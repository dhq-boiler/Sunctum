using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface ISubQueryTransition<Return> where Return : class
    {

        Return SubQuery(ICloseSyntax<IConditionValueSyntax> subquery);

        Return SubQuery(IConditionValueSyntax subquery);

        Return SubQuery(IJoinConditionSyntax subquery);

        Return SubQuery(IOrderBySyntax subquery);

        Return SubQuery(IOrderByColumnSyntax subquery);
    }
}
