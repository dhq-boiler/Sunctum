
namespace Sunctum.ViewModels
{
    internal class ContentDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;
        private int _number;

        public ContentDocumentViewModel(string title)
        {
            _title = title;
            _number = s_internalNumber++;
        }

        public override string Title => _title;

        public override string ContentId => $"content-{_number}";

        public override bool CanClose => true;
    }
}
