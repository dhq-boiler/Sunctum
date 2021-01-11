

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface IRecentOpenedLibraryUpdating : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        IDataAccessManager DataAccessManager { get; set; }
    }
}
