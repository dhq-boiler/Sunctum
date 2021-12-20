

using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface ITagRemoving : IAsyncTaskMaker
    {
        Lazy<ITagManager> TagManager { get; set; }

        IEnumerable<string> TagNames { get; set; }
    }
}
