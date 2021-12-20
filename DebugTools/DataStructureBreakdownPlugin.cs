

using NLog;
using Prism.Commands;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Yaml;
using Sunctum.Plugin;
using Sunctum.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace DebugTools
{
#if DEBUG

    [Export(typeof(IAddMenuPlugin))]
    public class DataStructureBreakdownPlugin : IAddMenuPlugin
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private static readonly ICommand s_command = new DelegateCommand<PluginMenuParameter>((p) =>
        {
            var parameter = (p.CommandParameter as IEnumerable<EntryViewModel>).Filter(p.CallFrom);
            var yaml = YamlConverter.ToYaml(parameter);
            TreeViewDialog dialog = new TreeViewDialog(yaml);
            dialog.Title = "Data Structure Breakdown";
            dialog.ShowDialog();
        },
        (p) =>
        {
            return true;
        });

        public FrameworkElement GetMenu(MenuType callFrom, Func<object> commandParameter)
        {
            return new PluginMenuItem("[DEBUG]Data Structure Breakdown", s_command, commandParameter, callFrom);
        }

        public MenuType Where()
        {
            return MenuType.MainWindow_Author_ContextMenu
                 | MenuType.MainWindow_Book_ContextMenu
                 | MenuType.MainWindow_Page_ContextMenu
                 | MenuType.MainWindow_Tag_ContextMenu;
        }
    }
#endif //DEBUG
}
