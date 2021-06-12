using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public class ExceptionOccurredEventArgs : EventArgs
    {
        public ExceptionOccurredEventArgs()
        {
        }

        public ExceptionOccurredEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; set; }
    }
}
