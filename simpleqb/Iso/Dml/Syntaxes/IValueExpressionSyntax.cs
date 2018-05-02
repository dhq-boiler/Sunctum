

using simpleqb.Core;
using simpleqb.Iso.Dml.Transitions;

namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IValueExpressionSyntax : ISyntaxBase, IUpdateColumnTransition, ISql
    {
        IWhereSyntax<ISinkStateSyntax, IOperatorSyntax<ISinkStateSyntax>, IIsSyntax<ISinkStateSyntax>> Where { get; }
    }
}
