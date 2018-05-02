

using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Data.DaoFacade
{
    public class QueryFailedException : Exception
    {
        public IEnumerable<Exception> InnerExceptions { get; set; }

        public QueryFailedException()
            : base()
        { }

        public QueryFailedException(string message)
            : base()
        { }

        public QueryFailedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public QueryFailedException(IEnumerable<Exception> innerExceptions)
            : base()
        {
            InnerExceptions = innerExceptions;
        }

        public QueryFailedException(string message, IEnumerable<Exception> innerExceptions)
            : base()
        {
            InnerExceptions = innerExceptions;
        }
    }
}
