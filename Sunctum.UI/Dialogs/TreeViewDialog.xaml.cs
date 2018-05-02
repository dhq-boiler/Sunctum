

using Sunctum.UI.ViewModel;
using System.Windows;

namespace Sunctum.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for TreeViewDialog.xaml
    /// </summary>
    public partial class TreeViewDialog : Window
    {
        public TreeViewDialog(string yamlString)
        {
            InitializeComponent();
            DataContext = new TreeViewDialogViewModel(yamlString);
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
