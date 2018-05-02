
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IIsSyntax<R> : ISyntaxBase where R : class
    {
        R Null { get; }

        R NotNull { get; }
    }
}
