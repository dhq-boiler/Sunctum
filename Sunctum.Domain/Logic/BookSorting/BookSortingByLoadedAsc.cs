

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Sunctum.Domain.Logic.BookSorting
{
    internal class BookSortingByLoadedAsc : IBookSorting
    {
        public IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource)
        {
            Contract.Requires(loadedSource != null);
            return loadedSource;
        }
    }
}
