

using Prism.Commands;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Yaml;
using Sunctum.Plugin;
using Sunctum.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace DebugTools
{
#if DEBUG

    [Export(typeof(IPlugin))]
    public class CopyAsYamlFormatPlugin : IPlugin
    {
        private static readonly ICommand s_command = new DelegateCommand<PluginMenuParameter>((p) =>
        {
            var parameter = (p.CommandParameter as IEnumerable<EntryViewModel>).Filter(p.CallFrom);
            var yaml = YamlConverter.ToYaml(parameter);
            Clipboard.SetText(yaml);
        },
        (p) =>
        {
            return true;
        });

        public FrameworkElement GetMenu(MenuType callFrom, Func<object> commandParameter)
        {
            return new PluginMenuItem("[DEBUG]このレコードと子レコードをYAML形式でコピーする", s_command, commandParameter, callFrom);
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
