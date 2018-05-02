

using Sunctum.UI.Controls;
using Sunctum.ViewModels;
using System;
using System.Windows;

namespace Sunctum.Views
{
    /// <summary>
    /// Interaction logic for PreferencesDialog.xaml
    /// </summary>
    public partial class PreferencesDialog : Window
    {
        public PreferencesDialogViewModel PreferencesVM { get; private set; }

        public PreferencesDialog()
        {
            InitializeComponent();

            PreferencesVM = new PreferencesDialogViewModel();
            this.DataContext = PreferencesVM;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void reference_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderSelectDialog();
            dialog.InitialDirectory = WorkingDirectory.Text;

            if (dialog.ShowDialog() == true)
            {
                WorkingDirectory.Text = dialog.FileName;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            PreferencesVM.CheckUpdate_WorkingDirectory();
            PreferencesVM.CheckUpdate_ConnectionString();
            PreferencesVM.CheckUpdate_LockFileInImporting();

            PreferencesVM.CheckUpdate_BookListViewItemAuthorHeight();
            PreferencesVM.CheckUpdate_BookListViewItemImageHeight();
            PreferencesVM.CheckUpdate_BookListViewItemTitleHeight();
            PreferencesVM.CheckUpdate_BookListViewItemWidth();

            PreferencesVM.CheckUpdate_BookListViewItemMarginLeft();
            PreferencesVM.CheckUpdate_BookListViewItemMarginTop();
            PreferencesVM.CheckUpdate_BookListViewItemMarginRight();
            PreferencesVM.CheckUpdate_BookListViewItemMarginBottom();

            PreferencesVM.CheckUpdate_ContentListViewItemImageHeight();
            PreferencesVM.CheckUpdate_ContentListViewItemTitleHeight();
            PreferencesVM.CheckUpdate_ContentListViewItemWidth();

            PreferencesVM.CheckUpdate_ContentListViewItemMarginLeft();
            PreferencesVM.CheckUpdate_ContentListViewItemMarginTop();
            PreferencesVM.CheckUpdate_ContentListViewItemMarginRight();
            PreferencesVM.CheckUpdate_ContentListViewItemMarginBottom();

            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
