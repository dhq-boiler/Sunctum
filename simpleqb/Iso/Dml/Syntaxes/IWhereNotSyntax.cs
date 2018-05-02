
using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IWhereNotSyntax<R> : ISyntaxBase where R : class
    {
        IExistsSyntax<R> Exists { get; }
    }
}
