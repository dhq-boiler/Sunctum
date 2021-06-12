

using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    public class CloseWindowAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            Window.GetWindow(AssociatedObject).Close();
        }
    }
}
