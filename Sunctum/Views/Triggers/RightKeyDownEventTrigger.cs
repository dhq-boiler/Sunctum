

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class RightKeyDownEventTrigger : EventTrigger
    {
        public RightKeyDownEventTrigger()
            : base("KeyDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Right)
            {
                e.Handled = true;
                InvokeActions(eventArgs);
            }
        }
    }
}
