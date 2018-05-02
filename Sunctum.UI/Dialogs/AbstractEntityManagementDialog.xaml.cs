

using System.Windows;

namespace Sunctum.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for EntityManagementDialog.xaml
    /// </summary>
    public abstract partial class AbstractEntityManagementDialog : Window
    {
        public AbstractEntityManagementDialog()
        {
            InitializeComponent();
        }

        protected abstract void Close_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void Update_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void Revert_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void Remove_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void Add_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void Integrate_Button_Click(object sender, RoutedEventArgs e);

        protected abstract void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e);

        protected abstract void NewName_Text_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e);

        protected abstract void NewName_Text_KeyDown(object sender, System.Windows.Input.KeyEventArgs e);

        protected abstract void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e);

        protected abstract void SelectedName_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e);
    }
}
