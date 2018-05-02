

using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class InnerSyntax : SyntaxBase, IJoinTypeSyntax
    {
        internal InnerSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public IJoinTableSyntax Join(string tableName)
        {
            return new JoinTableSyntax(this, tableName);
        }

        public IJoinTableSyntax Join(string tableName, string tableAlias)
        {
            return new JoinTableSyntax(this, tableName, tableAlias);
        }

        public override string Represent()
        {
            return "INNER";
        }
    }
}
