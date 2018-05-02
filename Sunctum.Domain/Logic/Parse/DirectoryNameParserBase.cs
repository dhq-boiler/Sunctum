

using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sunctum.Domain.Logic.Parse
{
    public abstract class DirectoryNameParserBase : BindableBase, IDirectoryNameParser
    {
        public List<string> Tags { get; protected set; } = new List<string>();

        public string Author { get; protected set; }

        public string Title { get; protected set; }

        public bool HasTags { get { return Tags != null && Tags.Count() > 0; } }

        public bool HasAuthor { get { return !string.IsNullOrWhiteSpace(Author); } }

        public bool HasTitle { get { return !string.IsNullOrWhiteSpace(Title); } }

        public abstract string Pattern { get; set; }

        public virtual bool Match(string directoryName)
        {
            Regex regex = new Regex(Pattern);
            return regex.IsMatch(directoryName);
        }

        public virtual void Parse(string directoryName)
        {
            Regex regex = new Regex(Pattern);
            if (regex.IsMatch(directoryName))
            {
                var mc = regex.Match(directoryName);

                var tagStrs = mc.Groups["tag"].Value.Split(',');
                foreach (var tagstr in tagStrs)
                {
                    var trim = tagstr.Trim();
                    if (trim.Count() > 0)
                    {
                        Tags.Add(trim);
                    }
                }

                var authorStr = mc.Groups["author"].Value;
                Author = string.IsNullOrWhiteSpace(authorStr) ? null : authorStr;

                var titleStr = mc.Groups["title"].Value;
                Title = string.IsNullOrWhiteSpace(titleStr) ? null : titleStr.Trim();
            }
            else
            {
                throw new NotMatchException(directoryName);
            }
        }
    }
}
