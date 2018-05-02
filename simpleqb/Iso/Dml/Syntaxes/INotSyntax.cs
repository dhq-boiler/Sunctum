
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface INotSyntax : ISyntaxBase
    {
        IInSyntax In { get; }
    }
}
