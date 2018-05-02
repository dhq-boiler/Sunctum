

using Sunctum.Domain.Models.Managers;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface ITagRemoving : IAsyncTaskMaker
    {
        ITagManager TagManager { get; set; }

        IEnumerable<string> TagNames { get; set; }
    }
}
