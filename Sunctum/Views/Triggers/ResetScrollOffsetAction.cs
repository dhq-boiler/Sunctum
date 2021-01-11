

using Sunctum.UI.Controls;
using System.Windows.Interactivity;

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
