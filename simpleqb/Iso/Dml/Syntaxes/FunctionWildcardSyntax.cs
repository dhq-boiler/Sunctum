using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class FunctionAsteriskSyntax : SyntaxBase, INoMarginLeftSyntax
    {
        internal FunctionAsteriskSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public override string Represent()
        {
            return "*";
        }
    }
}
