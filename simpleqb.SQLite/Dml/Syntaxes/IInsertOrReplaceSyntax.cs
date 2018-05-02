using simpleqb.Core;
using simpleqb.Dml.Syntaxes;

namespace simpleqb.SQLite.Dml.Syntaxes
{
    public interface IInsertOrReplaceSyntax : ISyntaxBase
    {
        IIntoSyntax Into { get; }
    }
}