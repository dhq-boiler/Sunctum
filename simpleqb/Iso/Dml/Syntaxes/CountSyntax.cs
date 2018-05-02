using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class CountSyntax : SyntaxBase
    {
        internal CountSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public override string Represent()
        {
            return "COUNT";
        }
    }
}
