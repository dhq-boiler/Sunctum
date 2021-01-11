

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Extensions
{
    public static class UILogicExtensions
    {
        public static T LoopNext<T>(this IEnumerable<T> collection, T current)
        {
            int currentIndex = Array.IndexOf(collection.ToArray(), current);
            bool hasNext = collection.Count() > currentIndex + 1;
            if (hasNext)
            {
                return collection.ElementAt(currentIndex + 1);
            }
            else
            {
                return collection.First();
            }
        }

        public static T LoopPrevious<T>(this IEnumerable<T> collection, T current)
        {
            int currentIndex = Array.IndexOf(collection.ToArray(), current);
            bool hasPrevious = currentIndex - 1 >= 0;
            if (hasPrevious)
            {
                return collection.ElementAt(currentIndex - 1);
            }
            else
            {
                return collection.Last();
            }
        }
    }
}
