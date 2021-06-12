

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class EndKeyDownEventTrigger : EventTrigger
    {
        public EndKeyDownEventTrigger()
            : base("KeyDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.End)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
