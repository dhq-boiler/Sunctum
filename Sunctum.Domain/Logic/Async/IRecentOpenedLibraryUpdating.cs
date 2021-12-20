

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface IRecentOpenedLibraryUpdating : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        IDataAccessManager DataAccessManager { get; set; }
    }
}
