
using Prism.Services.Dialogs;

namespace Sunctum.ViewModels
{
    internal class SearchDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;
        private int _number;

        public SearchDocumentViewModel(IDialogService dialogService, string title)
            : base(dialogService)
        {
            _title = title;
            _number = s_internalNumber++;
        }

        public override string Title => _title;

        public override string ContentId => $"search-{_number}";

        public override bool CanClose => true;
    }
}
