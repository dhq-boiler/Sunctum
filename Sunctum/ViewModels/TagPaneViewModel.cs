

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
    internal class TagPaneViewModel : PaneViewModelBase, ITagPaneViewModel
    {
        private List<TagCountViewModel> _TagListBoxSelectedItems;
        private ObservableCollection<System.Windows.Controls.Control> _TagContextMenuItems;

        public override string Title
        {
            get { return "Tag"; }
        }

        public override string ContentId
        {
            get { return "tag"; }
        }

        public override bool CanClose => true;

        public List<TagCountViewModel> TagListBoxSelectedItems
        {
            get { return _TagListBoxSelectedItems; }
            set { SetProperty(ref _TagListBoxSelectedItems, value); }
        }

        public ObservableCollection<System.Windows.Controls.Control> TagContextMenuItems
        {
            get { return _TagContextMenuItems; }
            set { SetProperty(ref _TagContextMenuItems, value); }
        }

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        [Inject]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        [Inject]
        public ILibraryManager LibraryManager { get; set; }

        [Inject]
        public ITagManager TagManager { get; set; }

        public ICommand ClearResultSearchingByTagCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand DeleteTagEntryCommand { get; set; }

        public ICommand SearchByTagCommand { get; set; }

        public ICommand SwitchOrderCommand { get; set; }

        public TagPaneViewModel()
        {
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ClearResultSearchingByTagCommand = new DelegateCommand(() =>
            {
                ClearResultSearchingByTag();
            });
            CloseCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.DisplayTagPane = false;
            });
            DeleteTagEntryCommand = new DelegateCommand(() =>
            {
                var item = TagListBoxSelectedItems;
                TagManager.RemoveTag(item.Select(a => a.Tag.Name));
            });
            SearchByTagCommand = new DelegateCommand(() =>
            {
                var items = TagListBoxSelectedItems;
                SearchByTag(items);
            });
            SwitchOrderCommand = new DelegateCommand(() =>
            {
                TagManager.SwitchOrdering();
            });
        }

        public void ClearSelectedItems()
        {
            TagListBoxSelectedItems = new List<TagCountViewModel>();
        }

        public void BuildContextMenus_Tags()
        {
            TagContextMenuItems = new ObservableCollection<System.Windows.Controls.Control>();

            var menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "選択中のタグで検索",
                Command = SearchByTagCommand
            };
            TagContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "検索結果をクリア",
                Command = ClearResultSearchingByTagCommand
            };
            TagContextMenuItems.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            TagContextMenuItems.Add(menuitem);
        }

        private void ClearResultSearchingByTag()
        {
            LibraryManager.ClearSearchResult();
        }

        private void SearchByTag(IEnumerable<TagCountViewModel> items)
        {
            LibraryManager.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            TagManager.ShowBySelectedItems(LibraryManager, items.Select(itc => itc.Tag));
            HomeDocumentViewModel.ResetScrollOffset();
        }
    }
}
