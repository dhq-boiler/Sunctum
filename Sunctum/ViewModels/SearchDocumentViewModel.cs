
namespace Sunctum.ViewModels
{
    internal class SearchDocumentViewModel : DocumentViewModelBase
    {
        private static int s_internalNumber = 0;
        private string _title;

        public SearchDocumentViewModel(string title)
        {
            _title = title;
        }

        public override string Title => _title;

        public override string ContentId => $"search-{s_internalNumber}";

        public override bool CanClose => true;
    }
}
