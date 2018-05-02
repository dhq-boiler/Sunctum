

using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Plugin
{
    public static class Extensions
    {
        public static IEnumerable<MenuType> GetFlags(this MenuType target)
        {
            foreach (var value in Enum.GetValues(typeof(MenuType)))
            {
                if (((int)target & (int)value) == (int)value)
                {
                    yield return (MenuType)value;
                }
            }
        }


        public static IEnumerable<EntityBaseObjectViewModel> Filter(this IEnumerable<EntryViewModel> target, MenuType callFrom)
        {
            switch (callFrom)
            {
                case MenuType.MainWindow_Author_ContextMenu:
                    return target.Filter<AuthorViewModel>();
                case MenuType.MainWindow_Book_ContextMenu:
                    return target.Filter<BookViewModel>();
                case MenuType.MainWindow_Page_ContextMenu:
                    return target.Filter<PageViewModel>();
                case MenuType.MainWindow_Tag_ContextMenu:
                    return target.Filter<TagViewModel>();
            }

            throw new ArgumentException("callFrom parameter is invalid value.");
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<EntryViewModel> target)
        {
            return target.Where(x => x is T).Cast<T>();
        }
    }
}
