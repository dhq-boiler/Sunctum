

using System;
using System.Runtime.Serialization;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    [Serializable]
    public class NoEntityInsertedException : Exception
    {
        public NoEntityInsertedException()
        {
        }

        public NoEntityInsertedException(string message) : base(message)
        {
        }

        public NoEntityInsertedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoEntityInsertedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}