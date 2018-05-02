using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml
{
    public static class All
    {
        public static ICountParameterSyntax Column(string name)
        {
            return new AllColumnSyntax(name);
        }

        public static ICountParameterSyntax Column(string tableAlias, string name)
        {
            return new AllColumnSyntax(tableAlias, name);
        }
    }
}
