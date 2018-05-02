using simpleqb.Core;

namespace simpleqb.Vendor.SQLite.Dcl.Syntaxes
{
    internal class VacuumSyntax : SyntaxBase, IVacuumSyntax
    {
        internal VacuumSyntax()
            : base()
        { }

        internal VacuumSyntax(SyntaxBase syntax)
            : base(syntax)
        { }

        public override string Represent()
        {
            return "VACUUM";
        }

        public string ToSql()
        {
            return RelayQuery(this);
        }
    }
}
