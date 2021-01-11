

using Sunctum.Domain.ViewModels;
using Sunctum.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Sunctum.Views
{
    /// <summary>
    /// Interaction logic for BookPropertyDialog.xaml
    /// </summary>
    public partial class BookPropertyDialog : Window
    {
        public BookPropertyDialog(BookViewModel book)
        {
            InitializeComponent();
            (DataContext as BookPropertyDialogViewModel).Parent = this;
            (DataContext as BookPropertyDialogViewModel).SelectBook(book);
        }

        private void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            (DataContext as BookPropertyDialogViewModel).LoadAllAuthors();
        }

        private void Title_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    (DataContext as BookPropertyDialogViewModel).UpdateBook();
                    DialogResult = true;
                    break;
                case Key.Escape:
                    DialogResult = false;
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
