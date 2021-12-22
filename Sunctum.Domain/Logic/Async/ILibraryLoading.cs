

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryLoading : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        ITaskManager TaskManager { get; set; }

        ITagManager TagManager { get; set; }

        IAuthorManager AuthorManager { get; set; }
    }
}
