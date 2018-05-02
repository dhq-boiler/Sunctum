
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IJoinTableSyntax : ISyntaxBase
    {
        IJoinConditionSyntax Using(string[] columnNames);

        IOnSyntax On { get; }
    }
}
