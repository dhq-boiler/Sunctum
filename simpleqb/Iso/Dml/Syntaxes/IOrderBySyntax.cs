
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOrderBySyntax : ISyntaxBase, ISql
    {
        IOrderByColumnSyntax Column(string name);

        IOrderByColumnSyntax Column(string tableAlias, string name);
    }
}
