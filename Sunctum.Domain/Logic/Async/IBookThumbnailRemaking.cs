

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookThumbnailRemaking : IAsyncTaskMaker
    {
        IEnumerable<BookViewModel> TargetBooks { get; set; }
    }
}
