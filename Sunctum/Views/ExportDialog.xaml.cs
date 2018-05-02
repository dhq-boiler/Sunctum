

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace Sunctum.Views
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window
    {
        private ExportDialogViewModel _exportDialogVM;

        public ExportDialog(ILibraryManager libMng, BookViewModel[] books)
        {
            InitializeComponent();

            DataContext = _exportDialogVM = new ExportDialogViewModel(libMng, books);

            TextBox_OutputDirectory.TextVerifier = new Func<string, bool?>((s) => Directory.Exists(s));
        }

        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //ダイアログを閉じてから，タスクの非同期実行を行う
            DialogResult = true;
            await _exportDialogVM.RunExport();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Reference_Click(object sender, RoutedEventArgs e)
        {
            _exportDialogVM.ShowOpenFileDialog();
            Activate();
        }
    }
}
