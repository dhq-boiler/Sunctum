

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sunctum.Domain.Logic.PageSorting
{
    public class PageOrdering
    {
        public static BookViewModel OrderForward(PageViewModel page, BookViewModel book)
        {
            var bookContentPages = book.Contents;
            int index = bookContentPages.IndexOf(page);
            bookContentPages.Remove(page);
            bookContentPages.Insert(index + 1, page);
            Resetcontents(book, bookContentPages);
            return book;
        }

        public static BookViewModel OrderBackward(PageViewModel page, BookViewModel book)
        {
            var bookContentPages = book.Contents;
            int index = bookContentPages.IndexOf(page);
            bookContentPages.Remove(page);
            bookContentPages.Insert(index - 1, page);
            Resetcontents(book, bookContentPages);
            return book;
        }

        private static void Resetcontents(BookViewModel book, ObservableCollection<PageViewModel> bookContentPages)
        {
            List<PageViewModel> currentPageArray = new List<PageViewModel>(bookContentPages.ToArray());
            book.ResetContents(currentPageArray);
        }
    }
}
