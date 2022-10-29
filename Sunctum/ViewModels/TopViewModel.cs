using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.ViewModels
{
    public class TopViewModel : BindableBase, IDialogAware
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        public string Title => "Sunctum Launcher";

        public event Action<IDialogResult> RequestClose;

        public ReactiveCollection<RecentOpenedLibrary> RecentOpenedLibraryItems { get; set; }

        public ReactiveCommand<string> SelectRecentOpenedLibraryCommand { get; set; }

        [Dependency]
        public IDataAccessManager dataAccessManager { get; set; }

        public TopViewModel()
        {
            SelectRecentOpenedLibraryCommand = new ReactiveCommand<string>();
            SelectRecentOpenedLibraryCommand.Subscribe(path =>
            {
                var dialogResult = new DialogResult(ButtonResult.OK);
                dialogResult.Parameters.Add("WorkingDirectory", path);
                RequestClose.Invoke(dialogResult);
            })
            .AddTo(disposables);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            RecentOpenedLibraryItems = new ReactiveCollection<RecentOpenedLibrary>();
            var dao = dataAccessManager.AppDao.Build<RecentOpenedLibraryDao>();
            RecentOpenedLibraryItems.AddRange(dao.FindAll().OrderBy(x => x.AccessOrder));
        }
    }
}
