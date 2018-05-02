

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Delete : CrudBase, IQueryBuilder
    {
        public ITable From { get; private set; }

        public IWhere Where { get; private set; }

        public override IEnumerable<IRightValue> Parameters
        {
            get
            {
                List<IRightValue> ret = new List<IRightValue>();
                if (Where != null)
                {
                    var parameters = Where.Parameters;
                    foreach (var parameter in parameters)
                    {
                        if (parameter is PlaceholderRightValue)
                        {
                            var p = parameter as PlaceholderRightValue;
                            ret.Add(p);
                        }
                        else if (parameter is MultiplePlaceholderRightValue)
                        {
                            var p = parameter as MultiplePlaceholderRightValue;
                            ret.AddRange(p.OverwritedPlaceholderNames);
                        }
                        else if (parameter is SubqueryRightValue)
                        {
                            var p = parameter as SubqueryRightValue;
                            ret.AddRange(p.Subquery.Parameters);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }
                return ret;
            }
        }

        public Delete(ITable table)
        {
            From = table;
        }

        public IQueryBuilder AddOrderBy(string columnName, Ordering ordering = Ordering.Ascending)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhere(string columnName, object value, LogicalOperator and_or = LogicalOperator.And)
        {
            if (Where == null)
            {
                Where = new Where(columnName, value);
            }
            else
            {
                Where.Add(and_or, columnName, value);
            }
            return this;
        }

        public IQueryBuilder AddWhere(IIsNull columnName_is_null, LogicalOperator and_or = LogicalOperator.And)
        {
            if (Where == null)
            {
                Where = new Where(columnName_is_null);
            }
            else
            {
                Where.Add(and_or, columnName_is_null);
            }
            return this;
        }

        public IQueryBuilder AddWhere(IIn inoperator, LogicalOperator and_or = LogicalOperator.And)
        {
            if (Where == null)
            {
                Where = new Where(inoperator);
            }
            else
            {
                Where.Add(and_or, inoperator);
            }
            return this;
        }

        public IQueryBuilder AddWhere(IExists exists, LogicalOperator and_or = LogicalOperator.And)
        {
            if (Where == null)
            {
                Where = new Where(exists);
            }
            else
            {
                Where.Add(and_or, exists);
            }
            return this;
        }

        public IQueryBuilder AddWhereIn(string columnName, object[] values, LogicalOperator and_or = LogicalOperator.And)
        {
            if (Where == null)
            {
                Where = new Where(columnName, values);
            }
            else
            {
                Where.Add(and_or, columnName, values);
            }
            return this;
        }

        public IQueryBuilder AddWhereIn(string columnName, Select subquery)
        {
            Contract.Assert(Where == null);
            Where = new Where(columnName, subquery);
            return this;
        }

        public IQueryBuilder AddWhereIn(LogicalOperator and_or, string columnName, Select subquery)
        {
            Contract.Assert(Where != null);
            Where.Add(and_or, columnName, subquery);
            return this;
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
            string sql = $"DELETE";
            CheckDelimiter(ref sql);

            sql += $"FROM {(From.HasAttachedDatabaseAlias ? $"{From.AttachedDatabaseAlias}." : "")}{From.Name}";
            if (From.HasAlias)
            {
                sql += $" {From.Alias}";
            }

            if (Where != null)
            {
                CheckDelimiter(ref sql);
                sql += Where.ToSql(placeholderNameDictionary);
            }

            return sql;
        }

        public IQueryBuilder UpdateSet(params ColumnNameBindValuePair[] pairs)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddJoinOn(IJoin joinType, ITable rightTable, JoinOn joinOn)
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
