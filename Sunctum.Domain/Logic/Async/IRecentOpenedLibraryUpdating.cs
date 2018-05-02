

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface IRecentOpenedLibraryUpdating : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        IDataAccessManager DataAccessManager { get; set; }
    }
}
