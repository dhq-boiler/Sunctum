
using Prism.Services.Dialogs;

namespace Sunctum.ViewModels
{
    internal class ContentDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;
        private int _number;

        public ContentDocumentViewModel(IDialogService dialogService)
            : base(dialogService)
        {
            _number = s_internalNumber++;
        }

        public override string Title 
        {
            get { return _title; }
            set { _title = value; }
        }

        public override string ContentId => $"content-{_number}";

        public override bool CanClose => true;
    }
}
