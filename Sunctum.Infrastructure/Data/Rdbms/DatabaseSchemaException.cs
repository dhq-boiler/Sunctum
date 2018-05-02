

using System;
using System.Runtime.Serialization;

namespace Sunctum.Infrastructure.Data.Rdbms
{
    [Serializable]
    public class DatabaseSchemaException : Exception
    {
        public DatabaseSchemaException()
        {
        }

        public DatabaseSchemaException(string message) : base(message)
        {
        }

        public DatabaseSchemaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseSchemaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}