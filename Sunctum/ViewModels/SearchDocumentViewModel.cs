
using Prism.Services.Dialogs;

namespace Sunctum.ViewModels
{
    internal class SearchDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;
        private int _number;

        public SearchDocumentViewModel(IDialogService dialogService)
            : base(dialogService)
        {
            _number = s_internalNumber++;
        }

        public override string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public override string ContentId => $"search-{_number}";

        public override bool CanClose => true;
    }
}
