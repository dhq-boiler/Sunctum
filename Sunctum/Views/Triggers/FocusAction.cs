

using System.Windows;
using Microsoft.Xaml.Behaviors;

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
