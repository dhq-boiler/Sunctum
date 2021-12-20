

using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public interface ILibraryInitializing : IAsyncTaskMaker
    {
        IByteSizeCalculating ByteSizeCalculatingService { get; set; }
    }
}
