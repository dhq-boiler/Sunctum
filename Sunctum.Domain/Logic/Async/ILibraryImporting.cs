

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryImporting : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        string ImportLibraryFilename { get; set; }
    }
}
