

using Sunctum.ViewModels;
using System;
using System.Windows;

namespace Sunctum.Views
{
    /// <summary>
    /// Interaction logic for ErrorReportDialog.xaml
    /// </summary>
    public partial class ErrorReportDialog : Window
    {
        internal ErrorReportDialogViewModel VM { get; set; }
        public ErrorReportDialog(Exception exception)
        {
            InitializeComponent();

            VM = new ErrorReportDialogViewModel(exception);
            DataContext = VM;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ErrorReportDialogViewModel.TerminateApplication();
        }
    }
}
