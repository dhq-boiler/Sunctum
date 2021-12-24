

using Reactive.Bindings;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Async
{
    public interface IBookImporting : IAsyncTaskMaker
    {
        Lazy<ILibrary> LibraryManager { get; set; }

        IEnumerable<string> ObjectPaths { get; set; }

        string MasterDirectory { get; set; }
    }
}
