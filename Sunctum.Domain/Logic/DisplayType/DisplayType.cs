

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.DisplayType
{
    public abstract class DisplayType
    {
        public static readonly IDisplayType SideBySide = new DisplayTypeSideBySide();
        public static readonly IDisplayType Details = new DisplayTypeDetails();
    }
}
