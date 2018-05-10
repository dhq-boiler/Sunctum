

using System.Reflection;

namespace Sunctum.Domain.Extensions
{
    public static class Extensions
    {
        public static void CopyTo<T>(this T from, T to)
        {
            var fromPropertyInfos = from.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fromPropertyInfo in fromPropertyInfos)
            {
                var toPropertyInfo = to.GetType().GetProperty(fromPropertyInfo.Name);

                if (toPropertyInfo != null)
                {
                    toPropertyInfo.SetValue(to, fromPropertyInfo.GetValue(from));
                }
            }
        }
    }
}
