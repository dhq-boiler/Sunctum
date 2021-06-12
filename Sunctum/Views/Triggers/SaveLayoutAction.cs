

using Sunctum.Domain.Models;
using Microsoft.Xaml.Behaviors;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Sunctum.Views.Triggers
{
    internal class SaveLayoutAction : TriggerAction<DockingManager>
    {
        protected override void Invoke(object parameter)
        {
            var serializer = new XmlLayoutSerializer(AssociatedObject);
            serializer.Serialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
        }
    }
}
