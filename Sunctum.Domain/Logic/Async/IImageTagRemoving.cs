

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IImageTagRemoving : IAsyncTaskMaker
    {
        Lazy<ITagManager> TagManager { get; set; }

        IEnumerable<EntryViewModel> Entries { get; set; }

        IEnumerable<string> TagNames { get; set; }
    }
}
