

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryResetting : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }
    }
}
