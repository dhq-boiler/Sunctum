

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public class Where : DmlBase, IWhere, ISqlize
    {
        private IComparisonOperation _firstComparisonOperation;

        private List<Tuple<LogicalOperator, IComparisonOperation>> _nextComparisonOperation;

        public List<IRightValue> Parameters
        {
            get
            {
                List<IRightValue> ret = new List<IRightValue>();
                ret.Add(_firstComparisonOperation.RightValue);
                if (_nextComparisonOperation != null)
                {
                    foreach (var t in _nextComparisonOperation)
                    {
                        ret.Add(t.Item2.RightValue);
                    }
                }
                return ret;
            }
        }

        public Where(string columnName, object parameter)
        {
            var p = new PlaceholderRightValue(columnName, parameter);
            _firstComparisonOperation = new EqualityOperation(columnName, p);
        }

        public Where(string columnName, object[] parameters)
        {
            var p = new MultiplePlaceholderRightValue(columnName, parameters);
            _firstComparisonOperation = new In(columnName, p);
        }

        public Where(string columnName, Select subquery)
        {
            var p = new SubqueryRightValue(subquery);
            _firstComparisonOperation = new In(columnName, p);
        }

        public Where(IIsNull is_null)
        {
            _firstComparisonOperation = is_null;
        }

        public Where(IIn inoperator)
        {
            _firstComparisonOperation = inoperator;
        }

        public Where(IExists exists)
        {
            _firstComparisonOperation = exists;
        }

        public void Add(LogicalOperator and_or, string columnName, object parameter)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            var p = new PlaceholderRightValue(columnName, parameter);
            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, new EqualityOperation(columnName, p)));
        }

        public void Add(LogicalOperator and_or, string columnName, object[] parameters)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            var p = new MultiplePlaceholderRightValue(columnName, parameters);
            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, new In(columnName, p)));
        }

        public void Add(LogicalOperator and_or, string columnName, Select subquery)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            var p = new SubqueryRightValue(subquery);
            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, new In(columnName, p)));
        }

        public void Add(LogicalOperator and_or, IIsNull is_null)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, is_null));
        }

        public void Add(LogicalOperator and_or, IIn inoperator)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, inoperator));
        }

        public void Add(LogicalOperator and_or, IExists exists)
        {
            if (_nextComparisonOperation == null)
            {
                _nextComparisonOperation = new List<Tuple<LogicalOperator, IComparisonOperation>>();
            }

            _nextComparisonOperation.Add(new Tuple<LogicalOperator, IComparisonOperation>(and_or, exists));
        }

        public override string ToSql()
        {
            return ToSql(new Dictionary<string, int>());
        }

        public override string ToSql(Dictionary<string, int> placeholderNameDictionary)
        {
            if (_firstComparisonOperation != null)
            {
                if (_firstComparisonOperation is EqualityOperation)
                {
                    if ((_firstComparisonOperation as EqualityOperation).RightValue.Values.Count() == 0)
                    {
                        return string.Empty;
                    }
                }
            }

            string sql = $"WHERE {_firstComparisonOperation.ToSql(placeholderNameDictionary)}";

            if (_nextComparisonOperation != null)
            {
                CheckDelimiter(ref sql);
                foreach (var condition in _nextComparisonOperation)
                {
                    CheckDelimiter(ref sql);
                    sql += $"{Enum.GetName(typeof(LogicalOperator), condition.Item1).ToLower()} {condition.Item2.ToSql(placeholderNameDictionary)}";
                }
            }

            return sql;
        }
    }
}
