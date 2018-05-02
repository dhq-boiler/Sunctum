using simpleqb.Core;
using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Vendor.SQLite.Dml.Syntaxes
{
    internal class InsertOrReplaceSyntax : SyntaxBase, IInsertOrReplaceSyntax
    {
        public InsertOrReplaceSyntax()
        { }

        internal InsertOrReplaceSyntax(SyntaxBase syntax)
            : base(syntax)
        { }

        public IIntoSyntax Into { get { return new IntoSyntax(this); } }

        public override string Represent()
        {
            return "INSERT OR REPLACE";
        }
    }
}
