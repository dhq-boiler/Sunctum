
namespace Sunctum.ViewModels
{
    internal class ContentDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;

        public ContentDocumentViewModel(string title)
        {
            _title = title;
            s_internalNumber++;
        }

        public override string Title => _title;

        public override string ContentId => $"content-{s_internalNumber}";

        public override bool CanClose => true;
    }
}
