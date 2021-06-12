

using Prism.Interactivity.InteractionRequest;
using Sunctum.Domain.Models;
using Sunctum.ViewModels;
using System.IO;
using Microsoft.Xaml.Behaviors;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Sunctum.Views.Triggers
{
    internal class LoadLayoutAction : TriggerAction<DockingManager>
    {
        private MainWindowViewModel _mainWindowViewModel;

        protected override void Invoke(object parameter)
        {
            var eventArgs = parameter as InteractionRequestedEventArgs;
            var context = eventArgs.Context;
            _mainWindowViewModel = (MainWindowViewModel)context.Content;

            if (!File.Exists(Specifications.APP_LAYOUT_CONFIG_FILENAME))
            {
                var serializer = new XmlLayoutSerializer(AssociatedObject);
                serializer.Serialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
            }

            var deserializer = new XmlLayoutSerializer(AssociatedObject);
            deserializer.LayoutSerializationCallback += Deserializer_LayoutSerializationCallback;
            deserializer.Deserialize(Specifications.APP_LAYOUT_CONFIG_FILENAME);
        }

        private void Deserializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            switch (e.Model.ContentId)
            {
                case "home":
                    break;
                case "author":
                    _mainWindowViewModel.AuthorPane = (LayoutAnchorable)e.Model;
                    break;
                case "tag":
                    _mainWindowViewModel.TagPane = (LayoutAnchorable)e.Model;
                    break;
                case "information":
                    _mainWindowViewModel.InformationPane = (LayoutAnchorable)e.Model;
                    break;
            }
        }
    }
}
