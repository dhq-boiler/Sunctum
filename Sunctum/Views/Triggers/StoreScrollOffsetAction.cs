

using Prism.Interactivity.InteractionRequest;
using Sunctum.UI.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Sunctum.Views.Triggers
{
    internal class StoreScrollOffsetAction : TriggerAction<VirtualizingWrapPanel>
    {
        protected override void Invoke(object parameter)
        {
            var eventArgs = parameter as InteractionRequestedEventArgs;
            var notification = eventArgs.Context as Notification;
            var tuple = notification.Content as Tuple<Dictionary<Guid, Point>, Guid>;
            var dictionary = tuple.Item1;
            var bookId = tuple.Item2;
            dictionary[bookId] = AssociatedObject.GetOffset();
        }
    }
}
