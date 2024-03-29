﻿

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookExporting : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; }

        IEnumerable<BookViewModel> TargetBooks { get; set; }

        string DestinationDirectory { get; set; }

        bool IncludeTag { get; set; }
    }
}
