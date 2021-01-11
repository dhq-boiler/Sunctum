

using NLog;
using Prism.Mvvm;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Controls;
using System.Threading.Tasks;

namespace Sunctum.ViewModels
{
    public class ExportDialogViewModel : BindableBase
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private ILibrary _libMng;
        private BookViewModel[] _willExportBooks;
        private string _OutputDirectory;
        private bool _IncludeTabIntoFodlerName;

        public ExportDialogViewModel(ILibrary libMng, BookViewModel[] books)
        {
            _libMng = libMng;
            _willExportBooks = books;
        }

        public string OutputDirectory
        {
            get { return _OutputDirectory; }
            set { SetProperty(ref _OutputDirectory, value); }
        }

        public bool IncludeTabIntoFolderName
        {
            get { return _IncludeTabIntoFodlerName; }
            set { SetProperty(ref _IncludeTabIntoFodlerName, value); }
        }

        internal async Task RunExport()
        {
            await _libMng.ExportBooks(_willExportBooks, OutputDirectory, IncludeTabIntoFolderName);
        }

        internal void ShowOpenFileDialog()
        {
            var dialog = new FolderSelectDialog();
            dialog.InitialDirectory = OutputDirectory;

            if (dialog.ShowDialog() == true)
            {
                OutputDirectory = dialog.FileName;
            }
        }
    }
}
