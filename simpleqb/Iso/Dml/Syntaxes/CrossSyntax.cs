using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class CrossSyntax : SyntaxBase, ICrossSyntax
    {
        internal CrossSyntax(SyntaxBase syntax)
            : base(syntax)
        { }

        public IJoinConditionSyntax Join(string tableName)
        {
            return new JoinTableSyntax(this, tableName);
        }

        public IJoinConditionSyntax Join(string tableName, string tableAlias)
        {
            return new JoinTableSyntax(this, tableName, tableAlias);
        }

        public override string Represent()
        {
            return "CROSS";
        }
    }
}
