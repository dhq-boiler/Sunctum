

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Logic.Query
{
    public class Querying
    {
        public static bool IsDirty(ILibrary libVM, BookViewModel book)
        {
            BookViewModel refBook = libVM.BookSource.Where(b => b.ID.Equals(book.ID)).Single();
            return !book.Equals(refBook);
        }

        public static int BookContentsCount(Guid bookID, DataOperationUnit dataOpUnit = null)
        {
            PageDao dao = new PageDao();
            return dao.CountBy(new Dictionary<string, object>() { { "BookID", bookID } }, dataOpUnit?.CurrentConnection);
        }

        public static bool SortingSelected(IBookSorting currentSorting, string name)
        {
            var sorting = BookSorting.BookSorting.GetReferenceByName(name);
            var sortingType = sorting.GetType();
            return currentSorting.GetType().Equals(sortingType);
        }
    }
}
