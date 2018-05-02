using simpleqb.Core;
using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Vendor.SQLite.Dml.Syntaxes
{
    public interface IInsertOrReplaceSyntax : ISyntaxBase
    {
        IIntoSyntax Into { get; }
    }
}