﻿

using System;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class MouseWheelEventTrigger : EventTrigger
    {
        public MouseWheelEventTrigger()
            : base("MouseWheel")
        { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as MouseWheelEventArgs;
            if (e != null)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
