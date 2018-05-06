

using System;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    internal class DropEventTrigger : EventTrigger
    {
        public DropEventTrigger()
            : base("Drop")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as System.Windows.DragEventArgs;
            if (e != null)
            {
                e.Handled = true;
                InvokeActions(eventArgs);
            }
        }
    }
}
