

using System;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    [Serializable]
    public class FailedOpeningDatabaseException : Exception
    {
        internal FailedOpeningDatabaseException()
            : base()
        { }

        internal FailedOpeningDatabaseException(string message)
            : base(message)
        { }

        internal FailedOpeningDatabaseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
