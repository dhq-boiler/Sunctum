

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryInitializing : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        IByteSizeCalculating ByteSizeCalculatingService { get; set; }
    }
}
