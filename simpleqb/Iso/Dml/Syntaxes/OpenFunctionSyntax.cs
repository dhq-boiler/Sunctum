using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class OpenFunctionSyntax : SyntaxBase, INoMarginLeftSyntax
    {
        internal OpenFunctionSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public override string Represent()
        {
            return "(";
        }
    }
}
