
using System.Collections.Generic;

namespace Sunctum.UI.Core
{
    public class TreeEntry
    {
        public int Level { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

        public List<TreeEntry> Children { get; set; }

        public TreeEntry Parent { get; set; }

        public int IndentCount { get; set; }

        public bool IsArrayNode { get; set; }

        public TreeEntry()
        {
            Level = 0;
            Key = "ROOT";
        }

        public TreeEntry(TreeEntry parent, string key, int indentCount, bool isArrayNode)
        {
            Level = parent.Level + 1;
            Key = key;
            Parent = parent;
            Value = null;
            Children = new List<TreeEntry>();
            IndentCount = indentCount;
            IsArrayNode = isArrayNode;
        }
    }
}
