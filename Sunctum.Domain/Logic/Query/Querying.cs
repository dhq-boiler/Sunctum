

using Homura.ORM;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.AuthorSorting;
using Sunctum.Domain.Logic.BookSorting;
using Sunctum.Domain.Logic.ImageTagCountSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Logic.Query
{
    public class Querying
    {
        public static bool IsDirty(ILibrary libVM, BookViewModel book)
        {
            var dao = new BookDao();
            BookViewModel refBook = dao.FindBy(new Dictionary<string, object>() { { "ID", book.ID } }).Single().ToViewModel();
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

        public static bool SortingSelected(IImageTagCountSorting currentSorting, string name)
        {
            var sorting = ImageTagCountSorting.ImageTagCountSorting.GetReferenceByName(name);
            var sortingType = sorting.GetType();
            return currentSorting.GetType().Equals(sortingType);
        }

        public static bool SortingSelected(IAuthorSorting currentSorting, string name)
        {
            var sorting = AuthorSorting.AuthorSorting.GetReferenceByName(name);
            var sortingType = sorting.GetType();
            return currentSorting.GetType().Equals(sortingType);
        }
    }
}
