

using System.Linq;
using System.Reflection;

namespace Sunctum.Domain.Logic.AuthorSorting
{
    public abstract class AuthorSorting
    {
        public static readonly IAuthorSorting ByNameAsc = new AuthorSortingByNameAsc();
        public static readonly IAuthorSorting ByNameDesc = new AuthorSortingByNameDesc();
        public static readonly IAuthorSorting ByCountAsc = new AuthorSortingByCountAsc();
        public static readonly IAuthorSorting ByCountDesc = new AuthorSortingByCountDesc();

        public static IAuthorSorting GetReferenceByName(string propertyName)
        {
            var AuthorSortings = typeof(AuthorSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return AuthorSortings.Single(x => x.Name == propertyName).GetValue(typeof(IAuthorSorting)) as IAuthorSorting;
        }

        public static string GetPropertyName(IAuthorSorting sorting)
        {
            var name = sorting.GetType().Name;

            var fields = typeof(AuthorSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return fields.Single(x => ("AuthorSorting" + x.Name) == name).Name;
        }
    }
}
