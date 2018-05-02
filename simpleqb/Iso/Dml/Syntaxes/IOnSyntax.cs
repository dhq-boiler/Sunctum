
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOnSyntax : ISyntaxBase
    {
        ISearchCondition<IJoinConditionSyntax, IOperatorColumnSyntax<IJoinConditionSyntax>, IIsSyntax<IJoinConditionSyntax>> Column(string name);

        ISearchCondition<IJoinConditionSyntax, IOperatorColumnSyntax<IJoinConditionSyntax>, IIsSyntax<IJoinConditionSyntax>> Column(string tableAlias, string name);
    }
}
