using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Yaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace CopyAsYamlFormat.ViewModels
{
    public class CopyAsYamlFormatMenuViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private bool disposedValue;

        public CopyAsYamlFormatMenuViewModel()
        {
            CopyAsYamlFormatCommand.Subscribe(_ =>
            {
                var parameter = SelectManager.SelectedItems;
                var yaml = YamlConverter.ToYaml(parameter);
                Clipboard.SetText(yaml);
            })
            .AddTo(disposables);
        }
        
        [Dependency]
        public ISelectManager SelectManager { get; set; }

        public ReactiveCommand CopyAsYamlFormatCommand { get; } = new ReactiveCommand();

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
