

using Prism.Interactivity.InteractionRequest;
using Sunctum.UI.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    internal class RestoreScrollOffsetAction : TriggerAction<VirtualizingWrapPanel>
    {
        protected override void Invoke(object parameter)
        {
            var eventArgs = parameter as InteractionRequestedEventArgs;
            var notification = eventArgs.Context as Notification;
            var tuple = notification.Content as Tuple<Dictionary<Guid, Point>, Guid>;
            var dictionary = tuple.Item1;
            var bookId = tuple.Item2;
            if (dictionary.ContainsKey(bookId))
            {
                AssociatedObject.SetOffset(dictionary[bookId]);
            }
            else
            {
                AssociatedObject.ResetOffset();
            }
        }
    }
}
