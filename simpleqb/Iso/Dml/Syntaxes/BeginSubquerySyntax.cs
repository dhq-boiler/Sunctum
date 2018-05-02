

using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class BeginSubquerySyntax : RepeatableSyntax, INoMarginRightSyntax
    {
        internal BeginSubquerySyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        internal BeginSubquerySyntax(SyntaxBase syntaxBase, Delimiter prefix)
            : base(syntaxBase, prefix)
        { }

        public override string Represent()
        {
            return $"{Delimiter.ToString()}(";
        }
    }
}
