
namespace Sunctum.Domain.Data.Rdbms.SQLite
{
    public sealed class SpecificationDefaults
    {
        /// <summary>
        /// Maximum length of a string or BLOB
        /// </summary>
        public static readonly int SQLITE_MAX_LENGTH = 123456789;

        /// <summary>
        /// Maximum Number Of Columns
        /// </summary>
        public static readonly int SQLITE_MAX_COLUMN = 2000;

        /// <summary>
        /// Maximum Length Of An SQL Statement
        /// </summary>
        public static readonly int SQLITE_MAX_SQL_LENGTH = 1000000;

        /// <summary>
        /// Maximum Number Of Tables In A Join
        /// </summary>
        public static readonly int MAX_TABLE_JOIN = 64;

        /// <summary>
        /// Maximum Depth Of An Expression Tree
        /// </summary>
        public static readonly int SQLITE_MAX_EXPR_DEPTH = 1000;

        /// <summary>
        /// Maximum Number Of Arguments On A Function
        /// </summary>
        public static readonly int SQLITE_MAX_FUNCTION_ARG = 100;

        /// <summary>
        /// Maximum Number Of Terms In A Compound SELECT Statement
        /// </summary>
        public static readonly int SQLITE_MAX_COMPOUND_SELECT = 500;

        /// <summary>
        /// Maximum Length Of A LIKE Or GLOB Pattern
        /// </summary>
        public static readonly int SQLITE_MAX_LIKE_PATTERN_LENGTH = 50000;

        /// <summary>
        /// Maximum Number Of Host Parameters In A Single SQL Statement
        /// </summary>
        public static readonly int SQLITE_MAX_VARIABLE_NUMBER = 999;

        /// <summary>
        /// Maximum Depth Of Trigger Recursion
        /// </summary>
        public static readonly int SQLITE_MAX_TRIGGER_DEPTH = 1000;

        /// <summary>
        /// Maximum Number Of Attached Databases
        /// </summary>
        public static readonly int SQLITE_MAX_ATTACHED = 10;

        /// <summary>
        /// Maximum Number Of Pages In A Database File
        /// </summary>
        public static readonly int SQLITE_MAX_PAGE_COUNT = 1073741823;

        /// <summary>
        /// Maximum Number Of Rows In A Table
        /// </summary>
        public static readonly double MAX_ROWS_IN_TABLE = 1.8e+19;

        /// <summary>
        /// Maximum Database Size
        /// </summary>
        public static readonly double MAX_DATABASE_SIZE = 1.4e+14;
    }
}
