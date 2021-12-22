

using Sunctum.Domain.Models.Managers;
using System;
using static Sunctum.Domain.Logic.Async.BookHashing;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookHashing : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        UpdateRange Range { get; set; }
    }
}