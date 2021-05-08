using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.BookSorting
{
    internal class BookSortingByFingerPrintDesc : IBookSorting
    {
        public IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource)
        {
            var sorted = loadedSource.Where(x => x.FingerPrint != null).OrderByDescending(x => x.FingerPrint);
            var notSortable = loadedSource.Where(x => x.FingerPrint == null);
            return sorted.Union(notSortable);
        }
    }
}
