

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookLoading : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }
    }
}
