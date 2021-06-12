

using Sunctum.UI.Controls;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class ResetScrollOffsetAction : TriggerAction<VirtualizingWrapPanel>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.ResetOffset();
        }
    }
}
