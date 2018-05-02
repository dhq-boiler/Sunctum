

using System;
using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Update : CrudBase, IQueryBuilder
    {
        public ITable Table { get; private set; }

        public IEnumerable<ColumnNameBindValuePair> NameValuePairs { get; private set; }

        public IWhere Where { get; private set; }

        public override IEnumerable<IRightValue> Parameters
        {
            get
            {
                List<IRightValue> ret = new List<IRightValue>();
                foreach (var p in NameValuePairs)
                {
                    if (p is ColumnNameDirectValuePair) continue;
                    ret.Add(p.Parameter);
                }
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

        public Update(ITable table)
        {
            Table = table;
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
            string sql = $"UPDATE {(Table.HasAttachedDatabaseAlias ? $"{Table.AttachedDatabaseAlias}." : "")}{Table.Name}";
            CheckDelimiter(ref sql);
            sql += "SET";

            var queue = new Queue<ColumnNameBindValuePair>(NameValuePairs);
            while (queue.Count > 0)
            {
                CheckDelimiter(ref sql);
                sql += queue.Dequeue().ToSql(placeholderNameDictionary);
                if (queue.Count > 0)
                {
                    sql += DELIMITER_COMMA + DELIMITER_SPACE;
                }
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
            NameValuePairs = pairs;
            return this;
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
            throw new NotImplementedException();
        }
    }
}
