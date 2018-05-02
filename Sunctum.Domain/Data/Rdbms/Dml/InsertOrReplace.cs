

using System;
using System.Collections.Generic;
using System.Linq;
using Sunctum.Infrastructure.Data.Rdbms;
using Sunctum.Infrastructure.Data.Rdbms.Dml;

namespace Sunctum.Domain.Data.Rdbms.Dml
{
    public class InsertOrReplace : Insert
    {
        public InsertOrReplace(ITable table) : base(table)
        { }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = $"insert or replace into {Into.Name}";

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
            sql += "values";

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

            return sql;
        }
    }
}
