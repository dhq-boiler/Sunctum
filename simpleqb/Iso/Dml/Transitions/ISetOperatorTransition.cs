using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface ISetOperatorTransition
    {
        IUnionSyntax Union { get; }

        IExceptSyntax Except { get; }

        IIntersectSyntax Intersect { get; }
    }
}
