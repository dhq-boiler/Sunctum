

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryResetting : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }
    }
}
