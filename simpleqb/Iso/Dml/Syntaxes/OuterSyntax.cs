using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class OuterSyntax : SyntaxBase, IJoinTypeSyntax
    {
        internal OuterSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public IJoinTableSyntax Join(string tableName)
        {
            return new JoinTableSyntax(this, tableName);
        }

        public IJoinTableSyntax Join(string tableName, string tableAlias)
        {
            return new JoinTableSyntax(this, tableName);
        }

        public override string Represent()
        {
            return "OUTER";
        }
    }
}
