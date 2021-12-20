

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookRemoving : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        IEnumerable<BookViewModel> TargetBooks { get; set; }
    }
}
