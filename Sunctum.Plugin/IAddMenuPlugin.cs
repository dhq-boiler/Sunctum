

using System;
using System.Windows;

namespace Sunctum.Plugin
{
    public interface IAddMenuPlugin
    {
        FrameworkElement GetMenu(MenuType callFrom, Func<object> commandParameter);

        MenuType Where();
    }
}
