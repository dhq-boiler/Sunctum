﻿

using simpleqb.Core;
using System.Collections.Generic;
using System.Linq;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class ValuesSyntax : RepeatableSyntax, IValuesSyntax
    {
        internal ValuesSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }
        internal ValuesSyntax(SyntaxBase syntaxBase, Delimiter prefix)
            : base(syntaxBase, prefix)
        { }

        public IRowSyntax Row(IEnumerable<object> values)
        {
            return Row(values.ToArray());
        }

        public IRowSyntax Row(params object[] values)
        {
            return new RowSyntax(this, values, Delimiter.None);
        }

        public ISetClauseValueSyntax Value(object value)
        {
            return new SetClauseValueSyntax(this, value, Delimiter.OpenedParenthesis);
        }

        public ISetClauseValueSyntax Value(IEnumerable<object> values)
        {
            return Value(values.ToArray());
        }

        public ISetClauseValueSyntax Value(params object[] values)
        {
            ISetClauseValueSyntax ret = null;
            foreach (var value in values)
            {
                if (ret == null)
                {
                    ret = new SetClauseValueSyntax(this, value, Delimiter.OpenedParenthesis);
                }
                else
                {
                    ret = new SetClauseValueSyntax(ret as SyntaxBase, value, Delimiter.Comma);
                }
            }
            return ret;
        }

        public IRowSyntax SubQuery(ICloseSyntax<IConditionValueSyntax> subquery)
        {
            var begin = new BeginSubquerySyntax(this);
            var end = new EndSubquerySyntax(begin);
            end.Relay.AddRange((subquery as SyntaxBase).PassRelay());
            return end;
        }

        public IRowSyntax SubQuery(IConditionValueSyntax subquery)
        {
            var begin = new BeginSubquerySyntax(this);
            var end = new EndSubquerySyntax(begin);
            end.Relay.AddRange((subquery as SyntaxBase).PassRelay());
            return end;
        }

        public IRowSyntax SubQuery(IJoinConditionSyntax subquery)
        {
            var begin = new BeginSubquerySyntax(this);
            var end = new EndSubquerySyntax(begin);
            end.Relay.AddRange((subquery as SyntaxBase).PassRelay());
            return end;
        }

        public IRowSyntax SubQuery(IOrderBySyntax subquery)
        {
            var begin = new BeginSubquerySyntax(this);
            var end = new EndSubquerySyntax(begin);
            end.Relay.AddRange((subquery as SyntaxBase).PassRelay());
            return end;
        }

        public IRowSyntax SubQuery(IOrderByColumnSyntax subquery)
        {
            var begin = new BeginSubquerySyntax(this);
            var end = new EndSubquerySyntax(begin);
            end.Relay.AddRange((subquery as SyntaxBase).PassRelay());
            return end;
        }

        public override string Represent()
        {
            return "VALUES";
        }

        public IRowSyntax Rows(IEnumerable<IEnumerable<object>> rows)
        {
            return Rows(rows.Cast<object[]>().ToArray());
        }

        public IRowSyntax Rows(params object[][] rows)
        {
            IRowSyntax ret = null;
            foreach (var row in rows)
            {
                if (ret == null)
                {
                    ret = new RowSyntax(this, row);
                }
                else
                {
                    ret = new RowSyntax(ret as SyntaxBase, row, Delimiter.Comma);
                }
            }
            return ret;
        }
    }
}
