

using System.Collections.Generic;

namespace Sunctum.Domain.Logic.Parse
{
    public interface IDirectoryNameParser
    {
        string Pattern { get; set; }

        bool HasTags { get; }

        bool HasAuthor { get; }

        bool HasTitle { get; }

        List<string> Tags { get; }

        string Author { get; }

        string Title { get; }

        bool Match(string directoryName);

        void Parse(string directoryName);
    }
}
