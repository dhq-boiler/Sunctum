

using System;
using System.Runtime.Serialization;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    [Serializable]
    public class NotExistColumnException : Exception
    {
        public NotExistColumnException()
        {
        }

        public NotExistColumnException(string message) : base(message)
        {
        }

        public NotExistColumnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotExistColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}