

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryImporting : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        string ImportLibraryFilename { get; set; }
    }
}
