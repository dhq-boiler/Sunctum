

using System.Windows;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    public class FocusAction : TargetedTriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            Target.Focus();
        }
    }
}
