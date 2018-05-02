

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IPageRemoving : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        IEnumerable<PageViewModel> TargetPages { get; set; }
    }
}
