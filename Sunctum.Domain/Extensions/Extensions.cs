

using System.Linq;
using System.Reflection;

namespace Sunctum.Domain.Extensions
{
    public static class Extensions
    {
        public static void CopyTo<T>(this T from, T to)
        {
            var fromPropertyInfos = from.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fromPropertyInfo in fromPropertyInfos.Where(x => !x.Name.Equals("Item"))) //except indexer
            {
                var toPropertyInfo = to.GetType().GetProperty(fromPropertyInfo.Name);

                if (toPropertyInfo != null)
                {
                    var value = fromPropertyInfo.GetValue(from);
                    toPropertyInfo.SetValue(to, value);
                }
            }
        }
    }
}
