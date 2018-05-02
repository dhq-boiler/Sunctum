

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryResetting : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }
    }
}
