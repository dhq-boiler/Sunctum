using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Yaml;
using Sunctum.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DataStructureBreakdown.ViewModels
{
    public class DataStructureBreakdownMenuViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private bool disposedValue;

        public DataStructureBreakdownMenuViewModel()
        {
            DataStructureBreakdownCommand.Subscribe(_ =>
            {
                var parameter = SelectManager.GetCollection<EntityBaseObjectViewModel>();
                var yaml = YamlConverter.ToYaml(parameter);
                TreeViewDialog dialog = new TreeViewDialog(yaml);
                dialog.Title = "Data Structure Breakdown";
                dialog.ShowDialog();
            })
            .AddTo(disposables);
        }

        [Dependency]
        public ISelectManager SelectManager { get; set; }

        public ReactiveCommand DataStructureBreakdownCommand { get; } = new ReactiveCommand();

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
