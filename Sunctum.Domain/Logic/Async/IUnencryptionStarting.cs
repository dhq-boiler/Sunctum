

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface IUnencryptionStarting : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        string Password { get; set; }
    }
}
