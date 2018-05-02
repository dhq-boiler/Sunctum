
namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public interface IQueryBuilder : ISqlize, ICrud
    {
        IQueryBuilder SelectColumn(params string[] columns); //select

        IQueryBuilder UpdateSet(params ColumnNameBindValuePair[] pairs); //update

        IQueryBuilder InsertColumns(params string[] columnNames); //insert

        IQueryBuilder InsertValue(params object[] values); //insert

        IQueryBuilder InsertFromSubquery(Select subquery);

        IQueryBuilder AddWhere(string columnName, object value, LogicalOperator and_or = LogicalOperator.And); //select, update, delete

        IQueryBuilder AddWhere(IIsNull columnName_is_null, LogicalOperator and_or = LogicalOperator.And);

        IQueryBuilder AddWhere(IIn inoperator, LogicalOperator and_or = LogicalOperator.And);

        IQueryBuilder AddWhere(IExists exists, LogicalOperator and_or = LogicalOperator.And);

        IQueryBuilder AddWhereIn(string columnName, object[] values, LogicalOperator and_or = LogicalOperator.And); //select, update, delete

        IQueryBuilder AddWhereIn(string columnName, Select subquery);

        IQueryBuilder AddWhereIn(LogicalOperator and_or, string columnName, Select subquery);

        IQueryBuilder AddOrderBy(string columnName, Ordering ordering = Ordering.Ascending); //select

        IQueryBuilder LimitBy(int rowCount); //select

        IQueryBuilder LimitBy(int rowCount, int beginIndex); //select

        IQueryBuilder AddJoinOn(IJoin joinType, ITable rightTable, JoinOn joinOn);

        Select AsSubquery();

        IQueryBuilder GroupBy(params string[] columnNames);
    }
}
