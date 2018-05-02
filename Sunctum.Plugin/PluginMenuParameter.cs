

using System;

namespace Sunctum.Plugin
{
    public class PluginMenuParameter
    {
        public MenuType CallFrom { get; private set; }
        public Func<object> ReturnCommandParameter { get; private set; }

        public object CommandParameter { get; private set; }

        public PluginMenuParameter(MenuType callFrom)
        {
            CallFrom = callFrom;
        }

        public PluginMenuParameter(MenuType callFrom, object commandParameter)
            : this(callFrom)
        {
            CommandParameter = commandParameter;
        }

        public PluginMenuParameter(MenuType callFrom, Func<object> returnCommandParameter)
            : this(callFrom)
        {
            ReturnCommandParameter = returnCommandParameter;
        }
    }
}
