﻿

using Homura.ORM;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sunctum.Domain.Logic.BookSorting
{
    public abstract class BookSorting
    {
        public static readonly IBookSorting ByLoadedAsc = new BookSortingByLoadedAsc();
        public static readonly IBookSorting ByLoadedDesc = new BookSortingByLoadedDesc();
        public static readonly IBookSorting ByTitleAsc = new BookSortingByTitleAsc();
        public static readonly IBookSorting ByTitleDesc = new BookSortingByTitleDesc();
        public static readonly IBookSorting ByAuthorAsc = new BookSortingByAuthorAsc();
        public static readonly IBookSorting ByAuthorDesc = new BookSortingByAuthorDesc();
        public static readonly IBookSorting ByCoverBlueAsc = new BookSortingByCoverBlueAsc();
        public static readonly IBookSorting ByCoverBlueDesc = new BookSortingByCoverBlueDesc();
        public static readonly IBookSorting ByCoverGreenAsc = new BookSortingByCoverGreenAsc();
        public static readonly IBookSorting ByCoverGreenDesc = new BookSortingByCoverGreenDesc();
        public static readonly IBookSorting ByCoverRedAsc = new BookSortingByCoverRedAsc();
        public static readonly IBookSorting ByCoverRedDesc = new BookSortingByCoverRedDesc();
        public static readonly IBookSorting ByFingerPrintAsc = new BookSortingByFingerPrintAsc();
        public static readonly IBookSorting ByFingerPrintDesc = new BookSortingByFingerPrintDesc();

        public static IBookSorting GetReferenceByName(string propertyName)
        {
            var bookSortings = typeof(BookSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return bookSortings.Single(x => x.Name == propertyName).GetValue(typeof(IBookSorting)) as IBookSorting;
        }

        public static string GetPropertyName(IBookSorting sorting)
        {
            var name = sorting.GetType().Name;

            var fields = typeof(BookSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return fields.Single(x => ("BookSorting" + x.Name) == name).Name;
        }

        internal static void FillAuthor(IEnumerable<BookViewModel> loadedSource)
        {
            using (var dou = new DataOperationUnit())
            {
                dou.Open(ConnectionManager.DefaultConnection);
                var loading = loadedSource.Where(b => b.Author == null);
                foreach (var book in loading)
                {
                    BookLoading.LoadAuthor(book, dou);
                }
            }
        }

        internal static void FillFirstPage(IEnumerable<BookViewModel> loadedSource)
        {
            var notAvailable = loadedSource.Where(x => x.FirstPage == null);
            using (var dou = new DataOperationUnit())
            {
                dou.Open(ConnectionManager.DefaultConnection);
                foreach (var book in notAvailable)
                {
                    BookLoading.Load(book, dou);
                }
            }
        }
    }
}
