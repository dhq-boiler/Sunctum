

using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Models.Managers;
using System;

namespace Sunctum.Domain.Test.Logic.Async
{
    public class RecentOpenedLibraryUpdatingMock : AsyncTaskMakerBase, IRecentOpenedLibraryUpdating
    {
        public IDataAccessManager DataAccessManager { get; set; }

        public Lazy<ILibrary> LibraryManager { get; set; }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        { }
    }
}
