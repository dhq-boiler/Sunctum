

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookLoading : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }
    }
}
