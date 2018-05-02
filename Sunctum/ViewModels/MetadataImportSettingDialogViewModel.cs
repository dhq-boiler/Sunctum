

using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Properties;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace Sunctum.ViewModels
{
    internal class MetadataImportSettingDialogViewModel
    {
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        public ReadOnlyReactiveCollection<DirectoryNameParserViewModel> DirectoryNameParsers { get; }

        public ReactiveProperty<DirectoryNameParserViewModel> SelectedParser { get; } = new ReactiveProperty<DirectoryNameParserViewModel>();

        public ReactiveCommand UpCommand { get; }

        public ReactiveCommand DownCommand { get; }

        public ReactiveCommand AddCommand { get; } = new ReactiveCommand();

        public ReactiveCommand EditCommand { get; }

        public ReactiveCommand RemoveCommand { get; }

        public ReactiveCommand<Window> CloseCommand { get; } = new ReactiveCommand<Window>();

        public InteractionRequest<Notification> AddRequest { get; } = new InteractionRequest<Notification>();

        public InteractionRequest<Notification> EditRequest { get; } = new InteractionRequest<Notification>();

        public MetadataImportSettingDialogViewModel(IDirectoryNameParserManager directoryNameParserManager)
        {
            DirectoryNameParserManager = directoryNameParserManager;
            DirectoryNameParsers = DirectoryNameParserManager
                .Items
                .ToReadOnlyReactiveCollection(x => new DirectoryNameParserViewModel(DirectoryNameParserManager, x));
            UpCommand = SelectedParser
                .Select(x => x != null
                          && DirectoryNameParserManager.Items.Count >= 2
                          && GetIndex(x.Model) > 0)
                .ToReactiveCommand();
            UpCommand
                .Subscribe(() => Up());
            DownCommand = SelectedParser
                .Select(x => x != null
                          && DirectoryNameParserManager.Items.Count >= 2
                          && GetIndex(x.Model) < DirectoryNameParserManager.Items.Count - 1)
                .ToReactiveCommand();
            DownCommand
                .Subscribe(() => Down());
            AddCommand
                .Select(_ => new DirectoryNameParser())
                .Subscribe(x => AddRequest.Raise(new Notification
                {
                    Title = Resources.EditMetadataImportSettingDialogTitle_Add,
                    Content = x
                }));
            EditCommand = SelectedParser
                .Select(x => x != null)
                .ToReactiveCommand();
            EditCommand
                .Select(_ => SelectedParser.Value.Model)
                .Subscribe(x => EditRequest.Raise(new Notification
                {
                    Title = Resources.EditMetadataImportSettingDialogTitle_Edit,
                    Content = x
                }));
            RemoveCommand = SelectedParser
                .Select(x => x != null)
                .ToReactiveCommand();
            RemoveCommand
                .Subscribe(() => Remove());
            CloseCommand
                .Subscribe(dialog =>
                {
                    DirectoryNameParserManager.Save();
                    dialog.DialogResult = true;
                });
        }

        private void Up()
        {
            var index = GetIndex(SelectedParser.Value.Model);
            var from = DirectoryNameParserManager.Items[index];
            var to = DirectoryNameParserManager.Items[index - 1];
            SwapIndex(from, to);
            DirectoryNameParserManager.Items[index] = to;
            DirectoryNameParserManager.Items[index - 1] = from;
        }

        private void Down()
        {
            var index = GetIndex(SelectedParser.Value.Model);
            var from = DirectoryNameParserManager.Items[index];
            var to = DirectoryNameParserManager.Items[index + 1];
            SwapIndex(from, to);
            DirectoryNameParserManager.Items[index] = to;
            DirectoryNameParserManager.Items[index + 1] = from;
        }

        private void Remove()
        {
            DirectoryNameParserManager.Items.Remove(SelectedParser.Value.Model);
            SelectedParser.Value = null;
        }

        private static void SwapIndex(DirectoryNameParser from, DirectoryNameParser to)
        {
            var tempIndex = from.Priority;
            from.Priority = to.Priority;
            to.Priority = tempIndex;
        }

        private int GetIndex(DirectoryNameParser target)
        {
            return DirectoryNameParserManager.Items.IndexOf(target);
        }
    }
}
