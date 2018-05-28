

using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Models.Managers;

namespace Sunctum.Domain.Test.Logic.Async
{
    public class RecentOpenedLibraryUpdatingMock : AsyncTaskMakerBase, IRecentOpenedLibraryUpdating
    {
        public IDataAccessManager DataAccessManager { get; set; }

        public ILibrary LibraryManager { get; set; }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        { }
    }
}
