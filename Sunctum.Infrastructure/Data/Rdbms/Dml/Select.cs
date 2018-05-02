

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Select : CrudBase, IQueryBuilder
    {
        public static readonly string WILDCARD = "*";

        public IEnumerable<string> Columns { get; private set; }

        public ITable From { get; private set; }

        public List<IJoin> JoinList { get; private set; }

        public IWhere Where { get; private set; }

        public IOrderBy Order { get; private set; }

        public ILimit Limit { get; private set; }

        public GroupBy Group { get; private set; }

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
                        else if (parameter == null) //IsNull, IsNotNull
                        {
                            //Do nothing
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

        public Select(ITable table)
        {
            From = table;
        }

        public IQueryBuilder AddOrderBy(string columnName, Ordering ordering = Ordering.Ascending)
        {
            if (Order == null)
            {
                Order = new OrderBy();
            }
            Order.Add(columnName, ordering);
            return this;
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
            Limit = new Limit(rowCount);
            return this;
        }

        public IQueryBuilder LimitBy(int rowCount, int beginIndex)
        {
            Limit = new Limit(rowCount, beginIndex);
            return this;
        }

        public IQueryBuilder SelectColumn(params string[] columns)
        {
            if (columns == null)
            {
                Columns = new List<string>(new string[] { WILDCARD });
            }
            else
            {
                Columns = columns.ToList();
            }
            return this;
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = $"SELECT";
            CheckDelimiter(ref sql);

            Queue<string> queue = new Queue<string>(Columns);
            while (queue.Count > 0)
            {
                sql += queue.Dequeue();
                if (queue.Count > 0)
                {
                    sql += DELIMITER_COMMA + DELIMITER_SPACE;
                }
            }

            CheckDelimiter(ref sql);

            sql += $"FROM {(From.HasAttachedDatabaseAlias ? $"{From.AttachedDatabaseAlias}." : "")}{From.Name}";
            if (From.HasAlias)
            {
                sql += $" {From.Alias}";
            }

            if (JoinList != null)
            {
                foreach (var join in JoinList)
                {
                    CheckDelimiter(ref sql);
                    sql += join.ToSql(placeholderNameDictionary);
                }
            }

            if (Where != null)
            {
                CheckDelimiter(ref sql);
                sql += Where.ToSql(placeholderNameDictionary);
            }

            if (Group != null)
            {
                CheckDelimiter(ref sql);
                sql += Group.ToSql(placeholderNameDictionary);
            }

            if (Order != null)
            {
                CheckDelimiter(ref sql);
                sql += Order.ToSql(placeholderNameDictionary);
            }

            if (Limit != null)
            {
                CheckDelimiter(ref sql);
                sql += Limit.ToSql(placeholderNameDictionary);
            }

            return sql;
        }

        public IQueryBuilder UpdateSet(params ColumnNameBindValuePair[] pairs)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder AddJoinOn(IJoin joinType, ITable rightTable, JoinOn joinOn)
        {
            if (JoinList == null)
            {
                JoinList = new List<IJoin>();
            }

            joinType.RightTable = rightTable;
            joinOn.RightTable = rightTable;
            joinType.AddJoinOn(joinOn);
            JoinList.Add(joinType);
            return this;
        }

        public Select AsSubquery()
        {
            return this;
        }

        public IQueryBuilder GroupBy(params string[] columnNames)
        {
            Group = new GroupBy(columnNames);
            return this;
        }

        public IQueryBuilder InsertFromSubquery(Select subquery)
        {
            throw new NotImplementedException();
        }
    }
}
