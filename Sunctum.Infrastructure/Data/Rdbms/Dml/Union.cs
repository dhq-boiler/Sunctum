

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Union : CrudBase, IQueryBuilder
    {
        public IEnumerable<IQueryBuilder> OperandList { get; private set; }

        public override IEnumerable<IRightValue> Parameters
        {
            get
            {
                foreach (var operand in OperandList)
                {
                    foreach (var p in operand.Parameters)
                    {
                        yield return p;
                    }
                }
            }
        }

        public Union(params IQueryBuilder[] operands)
        {
            OperandList = new List<IQueryBuilder>(operands);
        }

        public IQueryBuilder AddJoinOn(IJoin joinType, ITable rightTable, JoinOn joinOn)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddOrderBy(string columnName, Ordering ordering = Ordering.Ascending)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder AddWhere(string columnName, object value, LogicalOperator and_or = LogicalOperator.And)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhere(IIsNull columnName_is_null, LogicalOperator and_or = LogicalOperator.And)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhere(IIn inoperator, LogicalOperator and_or = LogicalOperator.And)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhere(IExists exists, LogicalOperator and_or = LogicalOperator.And)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhereIn(string columnName, object[] values, LogicalOperator and_or = LogicalOperator.And)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhereIn(string columnName, Select subquery)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder AddWhereIn(LogicalOperator and_or, string columnName, Select subquery)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder InsertColumns(params string[] columnNames)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder InsertValue(params object[] values)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder LimitBy(int rowCount)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder LimitBy(int rowCount, int beginIndex)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder SelectColumn(params string[] columns)
        {
            throw new NotSupportedException();
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            Debug.Assert(OperandList != null);
            Debug.Assert(OperandList.Count() >= 2);

            string sql = "";
            Queue<IQueryBuilder> queue = new Queue<IQueryBuilder>(OperandList);

            while (queue.Count > 0)
            {
                var q = queue.Dequeue();

                if (q is Select && (q as Select).Where != null)
                {
                    if (sql.Trim().Count() > 0)
                    {
                        CheckDelimiter(ref sql);
                        sql += "UNION";
                        CheckDelimiter(ref sql);
                    }

                    sql += q.ToSql(placeholderNameDictionary);
                }
            }

            return sql;
        }

        public IQueryBuilder UpdateSet(params ColumnNameBindValuePair[] pairs)
        {
            throw new NotSupportedException();
        }

        public Select AsSubquery()
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder GroupBy(params string[] columnNames)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder InsertFromSubquery(Select subquery)
        {
            throw new NotSupportedException();
        }
    }
}
