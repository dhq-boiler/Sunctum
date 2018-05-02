

using System;
using System.Runtime.Serialization;

namespace Sunctum.Domain.Logic.Parse
{
    [Serializable]
    internal class NotMatchException : Exception
    {
        public NotMatchException()
        {
        }

        public NotMatchException(string message) : base(message)
        {
        }

        public NotMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}