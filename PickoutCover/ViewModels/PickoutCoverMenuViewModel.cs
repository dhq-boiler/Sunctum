using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace PickoutCover.ViewModels
{
    public class PickoutCoverMenuViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private bool disposedValue;

        public PickoutCoverMenuViewModel(IDialogService dialogService)
        {
            PickoutCoverCommand.Subscribe(dataContext =>
            {
                IDialogParameters dialogParameters = new DialogParameters();
                dialogParameters.Add("page", dataContext);
                IDialogResult dialogResult = new DialogResult();
                dialogService.ShowDialog(nameof(Views.PickoutCover), dialogParameters, ret => dialogResult = ret);
            })
            .AddTo(disposables);
        }

        [Dependency]
        public PickoutCoverViewModel PickoutCoverDialogViewModel { get; set; }

        public ReactiveCommand<object> PickoutCoverCommand { get; } = new ReactiveCommand<object>();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposables = null;
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
