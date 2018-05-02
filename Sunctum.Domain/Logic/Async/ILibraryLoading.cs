

using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryLoading : IAsyncTaskMaker
    {
        ILibraryManager LibraryManager { get; set; }

        ITaskManager TaskManager { get; set; }

        ITagManager TagManager { get; set; }

        IAuthorManager AuthorManager { get; set; }
    }
}
