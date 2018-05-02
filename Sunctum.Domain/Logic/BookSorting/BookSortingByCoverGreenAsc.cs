

using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Domain.Logic.BookSorting
{
    internal class BookSortingByCoverGreenAsc : IBookSorting
    {
        public IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource)
        {
            Contract.Requires(loadedSource != null);
            BookSorting.FillFirstPage(loadedSource);
            var available = loadedSource.Where(x => x.FirstPage != null && x.FirstPage.Image.Thumbnail != null);
            var sorted = available.OrderBy(x => ColorMapLoader.LoadColorMap(x.ID, x.FirstPage.Image.ID, x.FirstPage.Image.Thumbnail.AbsoluteMasterPath, CoverComparator.Color.Green));
            var notSortable = loadedSource.Where(x => x.FirstPage == null || x.FirstPage.Image.Thumbnail == null);
            return notSortable.Union(sorted);
        }
    }
}
