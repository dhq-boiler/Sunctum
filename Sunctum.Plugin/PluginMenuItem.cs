

using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.Plugin
{
    public class PluginMenuItem : MenuItem
    {
        public MenuType CallFrom { get; set; }

        public ICommand CustomCommand { get; set; }

        public Func<object> ReturnCommandParameter { get; set; }

        public PluginMenuItem()
            : base()
        { }

        public PluginMenuItem(string header, ICommand command, Func<object> commandParameter)
            : base()
        {
            Header = header;
            CustomCommand = command;
            ReturnCommandParameter = commandParameter;
        }

        public PluginMenuItem(string header, ICommand command, Func<object> commandParameter, MenuType callFrom)
            : this(header, command, commandParameter)
        {
            CallFrom = callFrom;
        }

        protected override void OnClick()
        {
            var commandParameter = ReturnCommandParameter?.Invoke();
            var parameter = new PluginMenuParameter(CallFrom, commandParameter);
            CustomCommand.Execute(parameter);

            base.OnClick();
        }
    }
}
