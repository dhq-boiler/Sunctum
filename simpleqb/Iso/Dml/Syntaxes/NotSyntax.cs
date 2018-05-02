

using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class NotSyntax : SyntaxBase, INotSyntax
    {
        internal NotSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public IInSyntax In { get { return new InSyntax(this); } }

        public override string Represent()
        {
            return "NOT";
        }
    }
}
