

using System;
using System.Runtime.Serialization;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    [Serializable]
    public class NotMatchColumnException : Exception
    {
        public NotMatchColumnException()
        {
        }

        public NotMatchColumnException(string message) : base(message)
        {
        }

        public NotMatchColumnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotMatchColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}