
using Sunctum.UI.Core;
using System.Collections.ObjectModel;

namespace Sunctum.UI.ViewModel
{
    internal class TreeViewDialogViewModel
    {
        public ObservableCollection<TreeEntry> Root { get; set; }

        public TreeViewDialogViewModel(string yamlString)
        {
            Root = new ObservableCollection<TreeEntry>(TreeGenerator.ParseYaml(yamlString).Children);
        }
    }
}
