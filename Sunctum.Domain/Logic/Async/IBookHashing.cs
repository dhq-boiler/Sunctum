

using Sunctum.Domain.Models.Managers;
using static Sunctum.Domain.Logic.Async.BookHashing;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookHashing : IAsyncTaskMaker
    {
        ILibrary LibraryManager { get; set; }

        UpdateRange Range { get; set; }
    }
}