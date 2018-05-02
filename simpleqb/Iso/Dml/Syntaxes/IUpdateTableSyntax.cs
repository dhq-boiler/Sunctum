using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IUpdateTableSyntax : ISyntaxBase
    {
        ISetSyntax Set { get; }
    }
}
