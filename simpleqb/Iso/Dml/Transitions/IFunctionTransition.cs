using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IFunctionTransition<Return> where Return : class
    {
        Return Count();

        Return Count(string name);

        Return Count(ICountParameterSyntax column);
    }
}
