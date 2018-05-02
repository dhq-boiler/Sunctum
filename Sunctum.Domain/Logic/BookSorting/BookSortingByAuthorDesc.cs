

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Domain.Logic.BookSorting
{
    internal class BookSortingByAuthorDesc : IBookSorting
    {
        public IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource)
        {
            Contract.Requires(loadedSource != null);
            BookSorting.FillAuthor(loadedSource);
            var sorted = loadedSource.Where(x => x.Author != null).OrderByDescending(x => x.Author.Name);
            var notSortable = loadedSource.Where(x => x.Author == null);
            return notSortable.Union(sorted);
        }
    }
}
