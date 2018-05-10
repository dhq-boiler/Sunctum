

using Ninject;
using Prism.Commands;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class AuthorPaneViewModel : PaneViewModelBase, IAuthorPaneViewModel
    {
        private ObservableCollection<System.Windows.Controls.Control> _AuthorContextMenuItems;
        private List<AuthorCountViewModel> _AuthorListBoxSelectedItems;

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        [Inject]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        [Inject]
        public ILibraryManager LibraryManager { get; set; }

        [Inject]
        public IAuthorManager AuthorManager { get; set; }

        public override string Title
        {
            get { return "Author"; }
        }

        public override string ContentId
        {
            get { return "author"; }
        }

        public override bool CanClose => true;

        public ObservableCollection<System.Windows.Controls.Control> AuthorContextMenuItems
        {
            get { return _AuthorContextMenuItems; }
            set { SetProperty(ref _AuthorContextMenuItems, value); }
        }

        public List<AuthorCountViewModel> AuthorListBoxSelectedItems
        {
            get { return _AuthorListBoxSelectedItems; }
            set { SetProperty(ref _AuthorListBoxSelectedItems, value); }
        }

        public ICommand ClearResultSearchingByAuthorCommand { get; set; }

        public ICommand SearchByAuthorCommand { get; set; }

        public ICommand SwitchOrderCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public AuthorPaneViewModel()
        {
            RegisterCommands();
            AuthorListBoxSelectedItems = new List<AuthorCountViewModel>();
        }

        public void ClearSelectedItems()
        {
            AuthorListBoxSelectedItems?.Clear();
        }

        private void RegisterCommands()
        {
            ClearResultSearchingByAuthorCommand = new DelegateCommand(() =>
            {
                ClearResultSearchingByAuthor();
            });
            SearchByAuthorCommand = new DelegateCommand(() =>
            {
                var items = AuthorListBoxSelectedItems;
                SearchByAuthor(items);
            });
            SwitchOrderCommand = new DelegateCommand(() =>
            {
                AuthorManager.SwitchOrdering();
            });
            CloseCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.DisplayAuthorPane = false;
            });
        }

        public void BuildContextMenus_Authors()
        {
            AuthorContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "選択中の作者で検索",
                Command = SearchByAuthorCommand
            };
            AuthorContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "検索結果をクリア",
                Command = ClearResultSearchingByAuthorCommand
            };
            AuthorContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            AuthorContextMenuItems.Add(menuitem);
        }

        private void ClearResultSearchingByAuthor()
        {
            LibraryManager.ClearSearchResult();
        }

        private void SearchByAuthor(IEnumerable<AuthorCountViewModel> items)
        {
            LibraryManager.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            AuthorManager.ShowBySelectedItems(LibraryManager, items.Select(ac => ac.Author));
            HomeDocumentViewModel.ResetScrollOffset();
        }
    }
}
