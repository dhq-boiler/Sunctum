﻿

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IImageTagAdding : IAsyncTaskMaker
    {
        ITagManager TagManager { get; set; }

        IEnumerable<EntryViewModel> Entries { get; set; }

        string TagName { get; set; }
    }
}
