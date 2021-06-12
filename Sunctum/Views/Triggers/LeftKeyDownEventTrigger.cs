

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class LeftKeyDownEventTrigger : EventTrigger
    {
        public LeftKeyDownEventTrigger()
            : base("KeyDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Left)
            {
                e.Handled = true;
                InvokeActions(eventArgs);
            }
        }
    }
}
