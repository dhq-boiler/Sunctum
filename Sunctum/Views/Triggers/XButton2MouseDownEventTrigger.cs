

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class XButton2MouseDownEventTrigger : EventTrigger
    {
        public XButton2MouseDownEventTrigger()
            : base("MouseDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as MouseEventArgs;
            if (e != null && e.XButton2 == MouseButtonState.Pressed)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
