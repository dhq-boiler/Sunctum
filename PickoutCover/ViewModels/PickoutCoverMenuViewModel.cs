using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.ViewModels;
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
            PickoutCoverCommand.Subscribe(_ =>
            {
                IDialogParameters dialogParameters = new DialogParameters();
                var selectedPages = SelectManager.GetCollection<PageViewModel>();
                dialogParameters.Add("page", selectedPages.First());
                IDialogResult dialogResult = new DialogResult();
                dialogService.ShowDialog(nameof(Views.PickoutCover), dialogParameters, ret => dialogResult = ret);
            })
            .AddTo(disposables);
        }

        [Dependency]
        public ISelectManager SelectManager { get; set; }

        public ReactiveCommand PickoutCoverCommand { get; } = new ReactiveCommand();

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
