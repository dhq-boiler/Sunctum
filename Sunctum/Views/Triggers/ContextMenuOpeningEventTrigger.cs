

using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    internal class ContextMenuOpeningEventTrigger : EventTrigger
    {
        public ContextMenuOpeningEventTrigger()
            : base("ContextMenuOpening")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as ContextMenuEventArgs;
            if (e != null)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
