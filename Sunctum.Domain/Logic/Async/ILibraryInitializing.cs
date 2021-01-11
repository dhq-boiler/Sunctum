

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryInitializing : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        IByteSizeCalculating ByteSizeCalculatingService { get; set; }
    }
}
