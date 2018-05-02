

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.BookSorting
{
    public interface IBookSorting
    {
        IEnumerable<BookViewModel> Sort(IEnumerable<BookViewModel> loadedSource);
    }
}
