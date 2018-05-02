

using System.Collections.Generic;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class InnerJoin : DmlBase, IJoin, ISqlize
    {
        private List<JoinOn> JoinOnList { get; set; }

        public ITable RightTable { get; set; }

        public InnerJoin()
        {
            JoinOnList = new List<JoinOn>();
        }

        public void AddJoinOn(params JoinOn[] joinOns)
        {
            JoinOnList.AddRange(joinOns);
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            string sql = $"INNER JOIN";
            CheckDelimiter(ref sql);
            sql += $"{RightTable.Name}";
            if (RightTable.HasAlias)
            {
                sql += $" {RightTable.Alias}";
            }

            var queue = new Queue<JoinOn>(JoinOnList);
            while (queue.Count > 0)
            {
                CheckDelimiter(ref sql);
                sql += queue.Dequeue().ToSql(placeholderNameDictionary);
            }
            return sql;
        }
    }
}
