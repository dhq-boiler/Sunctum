

using Ninject;
using Prism.Commands;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public class AuthorPaneViewModel : PaneViewModelBase, IAuthorPaneViewModel
    {
        private ObservableCollection<System.Windows.Controls.Control> _AuthorContextMenuItems;
        private List<AuthorCountViewModel> _AuthorListBoxSelectedItems;

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        [Inject]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        [Inject]
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public IAuthorManager AuthorManager { get; set; }

        [Inject, Named("AuthorSortingToBool")]
        public IValueConverter AuthorSortingToBool { get; set; }

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
                Header = "表示"
            };
            AuthorContextMenuItems.Add(menuitem);

            var sortMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "ソート"
            };
            menuitem.Items.Add(sortMenuitem);

            var childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "名前 昇順",
                Command = AuthorManager.SortByNameAscCommand,
            };
            SetBindingForIsChecked(childMenuitem, "ByNameAsc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "名前 降順",
                Command = AuthorManager.SortByNameDescCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByNameDesc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "カウント 昇順",
                Command = AuthorManager.SortByCountAscCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByCountAsc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "カウント 降順",
                Command = AuthorManager.SortByCountDescCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByCountDesc");
            sortMenuitem.Items.Add(childMenuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            AuthorContextMenuItems.Add(menuitem);
        }

        private void SetBindingForIsChecked(System.Windows.Controls.MenuItem childMenuitem, string converterParameter)
        {
            Binding binding = new Binding("AuthorManager.Sorting");
            binding.Converter = AuthorSortingToBool;
            binding.ConverterParameter = converterParameter;
            childMenuitem.SetBinding(System.Windows.Controls.MenuItem.IsCheckedProperty, binding);
        }

        private void ClearResultSearchingByAuthor()
        {
            var activeViewModel = MainWindowViewModel.ActiveDocumentViewModel;
            activeViewModel.BookCabinet.ClearSearchResult();
            AuthorManager.ClearSearchResult();
        }

        private void SearchByAuthor(IEnumerable<AuthorCountViewModel> items)
        {
            var activeViewModel = MainWindowViewModel.ActiveDocumentViewModel;
            activeViewModel.BookCabinet.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            AuthorManager.ShowBySelectedItems(items.Select(ac => ac.Author));
            HomeDocumentViewModel.ResetScrollOffset();
        }

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(AuthorManager.Sorting, name);
        }
    }
}
