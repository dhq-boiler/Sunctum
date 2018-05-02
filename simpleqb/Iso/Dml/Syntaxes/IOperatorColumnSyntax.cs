namespace simpleqb.Iso.Dml.Syntaxes
{
    public interface IOperatorColumnSyntax<R> : IOperatorSyntax<R> where R : class
    {
        R Column(string name);

        R Column(string tableAlias, string name);
    }
}
