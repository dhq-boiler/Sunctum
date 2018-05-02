

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IPageThumbnailRemaking : IAsyncTaskMaker
    {
        IEnumerable<PageViewModel> TargetPages { get; set; }
    }
}
