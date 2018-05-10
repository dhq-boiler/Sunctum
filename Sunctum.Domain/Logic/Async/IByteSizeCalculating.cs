

using Sunctum.Domain.Models.Managers;
using static Sunctum.Domain.Logic.Async.ByteSizeCalculating;

namespace Sunctum.Domain.Logic.Async
{
    public interface IByteSizeCalculating : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        UpdateRange Range { get; set; }
    }
}
