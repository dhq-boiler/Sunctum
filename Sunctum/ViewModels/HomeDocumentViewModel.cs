

using NLog;
using Prism.Services.Dialogs;
using Sunctum.Domain.ViewModels;

namespace Sunctum.ViewModels
{
    public class HomeDocumentViewModel : DocumentViewModelBase, IHomeDocumentViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region コマンド

        #endregion //コマンド

        public override string Title
        {
            get { return "Home"; }
            set { }
        }

        public override string ContentId
        {
            get { return "home"; }
        }

        public HomeDocumentViewModel(IDialogService dialogService)
            : base(dialogService)
        {
        }
    }
}
