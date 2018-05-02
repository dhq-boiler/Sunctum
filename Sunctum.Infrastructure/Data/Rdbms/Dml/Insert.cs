

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Insert : CrudBase, IQueryBuilder
    {
        public ITable Into { get; private set; }

        public List<IColumn> TargetColumns { get; private set; }

        public List<Tuple<IColumn, IRightValue>> Values { get; private set; }

        public Select Subquery { get; private set; }

        public List<List<Tuple<IColumn, IRightValue>>> Records { get; private set; }

        public override IEnumerable<IRightValue> Parameters
        {
            get
            {
                List<IRightValue> ret = new List<IRightValue>();
                if (Values != null)
                {
                    ret.AddRange(Values.Select(v => v.Item2));
                }
                else
                {
                    foreach (var record in Records)
                    {
                        ret.AddRange(record.Select(v => v.Item2));
                    }
                }
                return ret;
            }
        }

        public Insert(ITable table)
        {
            Into = table;
        }

        public IQueryBuilder AddOrderBy(string columnName, Ordering ordering = Ordering.Ascending)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
        }

        public IQueryBuilder AddWhereIn(LogicalOperator and_or, string columnName, Select subquery)
        {
            throw new NotSupportedException();
        }

        public IQueryBuilder InsertColumns(params string[] columnNames)
        {
            TargetColumns = new List<IColumn>();

            foreach (var columnName in columnNames)
            {
                var target = Into.Columns.Where(c => c.ColumnName == columnName);
                if (target.Count() != 1)
                {
                    throw new ArgumentException();
                }
                TargetColumns.Add(target.Single());
            }

            return this;
        }

        public IQueryBuilder InsertValue(params object[] values)
        {
            if (Values == null && Records == null)
            {
                Values = new List<Tuple<IColumn, IRightValue>>();

                for (int i = 0; i < values.Count(); ++i)
                {
                    IColumn column;
                    PlaceholderRightValue p;

                    if (TargetColumns != null)
                    {
                        column = TargetColumns[i];
                        p = new PlaceholderRightValue(column.ColumnName, values[i]);
                    }
                    else
                    {
                        column = Into.Columns.ElementAt(i);
                        p = new PlaceholderRightValue(Into.Columns.ElementAt(i).ColumnName, values[i]);
                    }

                    Values.Add(new Tuple<IColumn, IRightValue>(column, p));
                }
            }
            else
            {
                if (Records == null)
                {
                    Records = new List<List<Tuple<IColumn, IRightValue>>>();
                    Records.Add(Values);
                    Values = null;
                }

                List<Tuple<IColumn, IRightValue>> record = new List<Tuple<IColumn, IRightValue>>();
                for (int i = 0; i < values.Count(); ++i)
                {
                    IColumn column;
                    PlaceholderRightValue p;

                    if (TargetColumns != null)
                    {
                        column = TargetColumns[i];
                        p = new PlaceholderRightValue(column.ColumnName, values[i]);
                    }
                    else
                    {
                        column = Into.Columns.ElementAt(i);
                        p = new PlaceholderRightValue(Into.Columns.ElementAt(i).ColumnName, values[i]);
                    }
                    record.Add(new Tuple<IColumn, IRightValue>(column, p));
                }
                Records.Add(record);
            }

            Subquery = null;

            return this;
        }

        public IQueryBuilder InsertFromSubquery(Select subquery)
        {
            Subquery = subquery;
            Values = null;
            Records = null;

            return this;
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
            string sql = $"INSERT INTO {(Into.HasAttachedDatabaseAlias ? $"{Into.AttachedDatabaseAlias}." : "")}{Into.Name}";

            if (TargetColumns != null)
            {
                CheckDelimiter(ref sql);

                if (TargetColumns.Count > 0)
                {
                    sql += "(";
                }

                var queue = new Queue<IColumn>(TargetColumns);
                while (queue.Count > 0)
                {
                    sql += queue.Dequeue().ColumnName;
                    if (queue.Count > 0)
                    {
                        sql += ", ";
                    }
                }

                sql += ")";
            }

            CheckDelimiter(ref sql);
            if (Values != null && Records != null && Subquery != null)
            {
                throw new InvalidOperationException($"Insert statement must be specified VALUES statement XOR Subquery statement as inserting data.");
            }
            else if (Values != null || Records != null)
            {
                sql += "VALUES";

                if (Values != null && Values.Count() > 0)
                {
                    sql += "(";
                    var queue_v = new Queue<IRightValue>(Values.Select(v => v.Item2));
                    while (queue_v.Count > 0)
                    {
                        sql += $"{queue_v.Dequeue().ToSql()}";
                        if (queue_v.Count > 0)
                        {
                            sql += ", ";
                        }
                    }
                    sql += ")";
                }
                else
                {
                    var queue = new Queue<List<Tuple<IColumn, IRightValue>>>(Records);
                    while (queue.Count() > 0)
                    {
                        var record = queue.Dequeue();

                        sql += "(";
                        var queue_v = new Queue<IRightValue>(record.Select(v => v.Item2));
                        while (queue_v.Count > 0)
                        {
                            sql += $"{queue_v.Dequeue().ToSql(placeholderNameDictionary)}";
                            if (queue_v.Count() > 0)
                            {
                                sql += ", ";
                            }
                        }
                        sql += ")";

                        if (queue.Count > 0)
                        {
                            sql += ", ";
                        }
                    }
                }
            }
            else if (Subquery != null)
            {
                sql += Subquery.ToSql(placeholderNameDictionary);
            }
            else
            {
                throw new InvalidOperationException($"Insert statement must be specified VALUES statement or Subquery statement as inserting data.");
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
    }
}
