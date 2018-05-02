

using System.Linq;
using System.Reflection;

namespace Sunctum.Domain.Logic.ImageTagCountSorting
{
    public abstract class ImageTagCountSorting
    {
        public static readonly IImageTagCountSorting ByNameAsc = new ImageTagCountSortingByNameAsc();
        public static readonly IImageTagCountSorting ByNameDesc = new ImageTagCountSortingByNameDesc();
        public static readonly IImageTagCountSorting ByCountAsc = new ImageTagCountSortingByCountAsc();
        public static readonly IImageTagCountSorting ByCountDesc = new ImageTagCountSortingByCountDesc();

        public static IImageTagCountSorting GetReferenceByName(string propertyName)
        {
            var ImageTagCountSortings = typeof(ImageTagCountSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return ImageTagCountSortings.Single(x => x.Name == propertyName).GetValue(typeof(IImageTagCountSorting)) as IImageTagCountSorting;
        }

        public static string GetPropertyName(IImageTagCountSorting sorting)
        {
            var name = sorting.GetType().Name;

            var fields = typeof(ImageTagCountSorting).GetFields(BindingFlags.Static | BindingFlags.Public);
            return fields.Single(x => ("ImageTagCountSorting" + x.Name) == name).Name;
        }
    }
}
