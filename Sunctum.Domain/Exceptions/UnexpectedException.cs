using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Exceptions
{
    public class UnexpectedException : Exception
    {
        public UnexpectedException()
        {
        }

        public UnexpectedException(string message) : base(message)
        {
        }

        public UnexpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
