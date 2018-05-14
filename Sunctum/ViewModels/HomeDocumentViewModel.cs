

using NLog;

namespace Sunctum.ViewModels
{
    internal class HomeDocumentViewModel : DocumentViewModelBase, IHomeDocumentViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region コマンド

        #endregion //コマンド

        public override string Title
        {
            get { return "Home"; }
        }

        public override string ContentId
        {
            get { return "home"; }
        }

        public HomeDocumentViewModel()
        {
        }
    }
}
