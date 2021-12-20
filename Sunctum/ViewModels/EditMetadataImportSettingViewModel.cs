using Prism.Services.Dialogs;
using Reactive.Bindings;
using Sunctum.Domain.Logic.Parse;
using Sunctum.Properties;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Unity;

namespace Sunctum.ViewModels
{
    internal class EditMetadataImportSettingViewModel : IDialogAware
    {
        public Action FinishInteraction { get; set; }

        public ReactiveProperty<DirectoryNameParserViewModel> EditTarget { get; } = new ReactiveProperty<DirectoryNameParserViewModel>();

        [Dependency]
        public IDirectoryNameParserManager DirectoryNameParserManager { get; set; }

        public ReactiveCommand OpenBrowserCommand { get; } = new ReactiveCommand();

        public ReactiveCommand OkCommand { get; set; }

        public ReactiveCommand CancelCommand { get; set; } = new ReactiveCommand();

        public string Title => "Edit metadata import setting";

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

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
