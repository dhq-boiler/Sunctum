

using Sunctum.Domain.Models.Managers;
using System;
using static Sunctum.Domain.Logic.Async.ByteSizeCalculating;

namespace Sunctum.Domain.Logic.Async
{
    public interface IByteSizeCalculating : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        UpdateRange Range { get; set; }
    }
}
