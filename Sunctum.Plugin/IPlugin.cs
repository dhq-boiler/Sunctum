

using System;
using System.Windows;

namespace Sunctum.Plugin
{
    public interface IPlugin
    {
        FrameworkElement GetMenu(MenuType callFrom, Func<object> commandParameter);

        MenuType Where();
    }
}
