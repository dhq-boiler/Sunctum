using simpleqb.Core;
using simpleqb.Iso.Dml.Syntaxes;
using simpleqb.Vendor.SQLite.Dml.Syntaxes;

namespace simpleqb.Vendor.SQLite.Dml
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

        public static ILimitSyntax Limit(this IValueExpressionSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this ISetClauseValueSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }

        public static ILimitSyntax Limit(this IJoinConditionSyntax syntax, int count)
        {
            return new LimitSyntax(syntax as SyntaxBase, count);
        }
    }
}
