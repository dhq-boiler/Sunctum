

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.UI.Controls
{
    public class EasyEnterTextBox : TextBox
    {
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!IsFocused)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
                SelectAll();
                e.Handled = true;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            SelectAll();
            e.Handled = true;
        }
    }
}
