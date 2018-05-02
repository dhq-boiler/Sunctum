

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Domain.Logic.BookSorting
{
    internal class BookSortingByAuthorAsc : IBookSorting
    {
        public IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource)
        {
            Contract.Requires(loadedSource != null);
            BookSorting.FillAuthor(loadedSource);
            var sorted = loadedSource.Where(x => x.Author != null).OrderBy(x => x.Author.Name);
            var notSortable = loadedSource.Where(x => x.Author == null);
            return sorted.Union(notSortable);
        }
    }
}
