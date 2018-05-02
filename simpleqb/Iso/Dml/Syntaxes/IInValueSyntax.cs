
using simpleqb.Core;
using System.Collections.Generic;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IInValueSyntax : ISyntaxBase, ICloseSyntax<IConditionValueSyntax>
    {
        IInValueSyntax Value(object value);

        IInValueSyntax Value(params object[] values);

        IInValueSyntax Value(IEnumerable<object> values);
    }
}
