

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class DeleteKeyDownEventTrigger : EventTrigger
    {
        public DeleteKeyDownEventTrigger()
            : base("KeyDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Delete)
            {
                e.Handled = true;
                InvokeActions(eventArgs);
            }
        }
    }
}
