using simpleqb.Core;
using simpleqb.Dml.Syntaxes;
using simpleqb.SQLite.Dml.Syntaxes;

namespace simpleqb.SQLite
{
    public static class Extensions
    {
        public static ILimitSyntax Limit(this ICloseSyntax<IConditionValueSyntax> syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IConditionValueSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IOrderByColumnSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IRowSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this ISinkStateSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this ISubstituteSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IValueSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IJoinConditionSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }
    }
}
