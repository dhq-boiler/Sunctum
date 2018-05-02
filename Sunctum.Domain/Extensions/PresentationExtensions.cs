

using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Extensions
{
    public static class PresentationExtensions
    {
        public static string ArrayToString<T>(this IEnumerable<T> array)
        {
            string ret = "[";
            for (int i = 0; i < array.Count(); ++i)
            {
                var element = array.ElementAt(i);
                ret += $"'{element.ToString()}'";
                if (i + 1 < array.Count())
                {
                    ret += ", ";
                }
            }
            ret += "]";
            return ret;
        }
    }
}
