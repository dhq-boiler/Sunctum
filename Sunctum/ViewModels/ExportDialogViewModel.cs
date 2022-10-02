

using NLog;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Properties;
using Sunctum.UI.Controls;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.ViewModels
{
    public class ExportDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private BookViewModel[] _willExportBooks;
        private string _OutputDirectory;
        private bool _IncludeTabIntoFodlerName;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue;

        [Dependency]
        public ILibrary Library { get; set; }

        public ReactiveCommand OKCommand { get; } = new ReactiveCommand();
        public ReactiveCommand CancelCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ReferenceCommand { get; } = new ReactiveCommand();

        public ExportDialogViewModel()
        {
            OKCommand.Subscribe(async _ =>
            {
                await RunExport().ConfigureAwait(false);
                App.Current.Dispatcher.Invoke(() =>
                {
                    RequestClose.Invoke(new DialogResult(ButtonResult.OK));
                });
            })
            .AddTo(_disposables);
            CancelCommand.Subscribe(_ =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
            })
            .AddTo(_disposables);
            ReferenceCommand.Subscribe(_ =>
            {
                ShowOpenFileDialog();
            })
            .AddTo(_disposables);
            TextVerifier.Value = new Func<string, bool?>((s) => Directory.Exists(s));
        }

        public ReactivePropertySlim<Func<string, bool?>> TextVerifier { get; } = new ReactivePropertySlim<Func<string, bool?>>();

        public string OutputDirectory
        {
            get { return _OutputDirectory; }
            set { SetProperty(ref _OutputDirectory, value); }
        }

        public bool IncludeTabIntoFolderName
        {
            get { return _IncludeTabIntoFodlerName; }
            set { SetProperty(ref _IncludeTabIntoFodlerName, value); }
        }

        public string Title => Resources.ExportDialogTitle;

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
            _willExportBooks = parameters.GetValue<BookViewModel[]>("TargetBooks");
        }

        internal async Task RunExport()
        {
            await Library.ExportBooks(_willExportBooks, OutputDirectory, IncludeTabIntoFolderName).ConfigureAwait(false);
        }

        internal void ShowOpenFileDialog()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.InitialDirectory = OutputDirectory;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputDirectory = dialog.SelectedPath;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }

                _disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
