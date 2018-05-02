

using System;

namespace Sunctum.Exceptions
{
    [Serializable]
    internal class BrokenPageIndexException : Exception
    {
        internal BrokenPageIndexException()
            : base()
        { }

        internal BrokenPageIndexException(string message)
            : base(message)
        { }

        internal BrokenPageIndexException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
