
using System;

namespace Sunctum.Plugin
{
    [Flags]
    public enum MenuType
    {
        MainWindow_Book_ContextMenu = 0x1,
        MainWindow_Page_ContextMenu = 0x2,
        MainWindow_Author_ContextMenu = 0x4,
        MainWindow_Tag_ContextMenu = 0x8,
    }
}
