
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IInsertSyntax : ISyntaxBase
    {
        IIntoSyntax Into { get; }
    }
}
