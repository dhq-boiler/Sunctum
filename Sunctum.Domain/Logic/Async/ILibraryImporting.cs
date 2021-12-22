

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryImporting : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        string ImportLibraryFilename { get; set; }
    }
}
