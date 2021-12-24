using Sunctum.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Sunctum.Views
{
    /// <summary>
    /// BookProperty.xaml の相互作用ロジック
    /// </summary>
    public partial class BookProperty : UserControl
    {
        public BookProperty()
        {
            InitializeComponent();
        }

        private void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            (DataContext as BookPropertyDialogViewModel).LoadAllAuthors();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as BookPropertyDialogViewModel).Parent = this;
        }
    }
}
