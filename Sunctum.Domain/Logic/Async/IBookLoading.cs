

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookLoading : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }
    }
}
