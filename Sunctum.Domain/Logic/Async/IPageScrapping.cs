

using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IPageScrapping : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        string Title { get; set; }

        IEnumerable<PageViewModel> TargetPages { get; set; }

        string MasterDirectory { get; set; }
    }
}