

using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Parse
{
    internal static class Extensions
    {
        public static IEnumerable<DirectoryNameParser> ToService(this IEnumerable<Models.DirectoryNameParser> source)
        {
            foreach (var item in source)
            {
                yield return new DirectoryNameParser()
                {
                    Priority = item.Priority,
                    Pattern = item.Pattern
                };
            }
        }

        public static IEnumerable<Models.DirectoryNameParser> ToEntity(this IEnumerable<DirectoryNameParser> source)
        {
            foreach (var item in source)
            {
                yield return new Models.DirectoryNameParser()
                {
                    Priority = item.Priority,
                    Pattern = item.Pattern
                };
            }
        }
    }
}
