

using System;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    internal class XButton1MouseDownEventTrigger : EventTrigger
    {
        public XButton1MouseDownEventTrigger()
            : base("MouseDown")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as MouseEventArgs;
            if (e != null && e.XButton1 == MouseButtonState.Pressed)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
