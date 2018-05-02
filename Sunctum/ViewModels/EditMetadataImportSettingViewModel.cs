

using Ninject;
using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Properties;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Sunctum.ViewModels
{
    internal class EditMetadataImportSettingViewModel : IInteractionRequestAware
    {
        public Action FinishInteraction { get; set; }

        private INotification _notification;
        public INotification Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                _notification = value;
                this.EditTarget.Value?.Dispose();
                this.EditTarget.Value = new DirectoryNameParserViewModel(DirectoryNameParserManager, value.Content as DirectoryNameParser);
            }
        }

        public ReactiveProperty<DirectoryNameParserViewModel> EditTarget { get; } = new ReactiveProperty<DirectoryNameParserViewModel>();

        [Inject]
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        public ReactiveCommand OpenBrowserCommand { get; } = new ReactiveCommand();

        public ReactiveCommand OkCommand { get; set; }

        public ReactiveCommand CancelCommand { get; set; } = new ReactiveCommand();

        public EditMetadataImportSettingViewModel()
        {
            OpenBrowserCommand
                .Subscribe(() => Process.Start(new ProcessStartInfo(Resources.EditMetadataImportSettingDialog_PatternUri)));
            OkCommand = EditTarget
                .Where(x => x != null)
                .Select(_ => true)
                .ToReactiveCommand();
            OkCommand
                .Subscribe(_ =>
                {
                    EditTarget.Value.Commit();
                    FinishInteraction();
                });
            CancelCommand
                .Subscribe(_ =>
                {
                    FinishInteraction();
                });
        }
    }
}
