using Ninject;
using PickoutCover.View.Dialog;
using Prism.Commands;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Plugin;
using Sunctum.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PickoutCover
{
    [Export(typeof(IAddMenuPlugin))]
    public class PickoutCoverPlugin : IAddMenuPlugin
    {
        private ICommand _command;
        private ILibrary _libraryManager;

        [Inject]
        public PickoutCoverPlugin(ILibrary libraryManager)
        {
            _libraryManager = libraryManager;
            _command = new DelegateCommand<PluginMenuParameter>((p) =>
            {
                var parameter = (p.CommandParameter as IEnumerable<EntryViewModel>).Filter(p.CallFrom);
                var first = parameter.Cast<PageViewModel>().First();
                OpenPickOutCoverDialog(_libraryManager, first);
            },
            (p) =>
            {
                var parameter = (p?.CommandParameter as IEnumerable<EntryViewModel>)
                                  ?.Where(x => x is PageViewModel)
                                  ?.Cast<PageViewModel>();
                return parameter?.Count() == 1;
            });
        }

        private void OpenPickOutCoverDialog(ILibrary library, PageViewModel page)
        {
            PickOutCoverDialog dialog = new PickOutCoverDialog(page, library);
            dialog.ShowDialog();
        }

        public System.Windows.FrameworkElement GetMenu(MenuType callFrom, Func<object> commandParameter)
        {
            return new PluginMenuItem("表紙切り取りツール", _command, commandParameter, callFrom);
        }

        public MenuType Where()
        {
            return MenuType.MainWindow_Page_ContextMenu;
        }
    }
}
