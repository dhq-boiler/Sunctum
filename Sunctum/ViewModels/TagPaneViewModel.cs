﻿

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
    public class TagPaneViewModel : PaneViewModelBase, ITagPaneViewModel
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
        public ILibrary LibraryManager { get; set; }

        [Inject]
        public ITagManager TagManager { get; set; }

        [Inject, Named("TagSortingToBool")]
        public IValueConverter TagSortingToBool { get; set; }

        public ICommand ClearResultSearchingByTagCommand { get; set; }

        public ICommand DeleteTagEntryCommand { get; set; }

        public ICommand SearchByTagCommand { get; set; }

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
                Header = "表示"
            };
            TagContextMenuItems.Add(menuitem);

            var sortMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "ソート"
            };
            menuitem.Items.Add(sortMenuitem);

            var childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "名前 昇順",
                Command = TagManager.SortByNameAscCommand,
            };
            SetBindingForIsChecked(childMenuitem, "ByNameAsc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "名前 降順",
                Command = TagManager.SortByNameDescCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByNameDesc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "カウント 昇順",
                Command = TagManager.SortByCountAscCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByCountAsc");
            sortMenuitem.Items.Add(childMenuitem);

            childMenuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "カウント 降順",
                Command = TagManager.SortByCountDescCommand
            };
            SetBindingForIsChecked(childMenuitem, "ByCountDesc");
            sortMenuitem.Items.Add(childMenuitem);

            menuitem = new System.Windows.Controls.MenuItem()
            {
                Header = "Ex",
            };
            TagContextMenuItems.Add(menuitem);
        }

        private void SetBindingForIsChecked(System.Windows.Controls.MenuItem childMenuitem, string converterParameter)
        {
            Binding binding = new Binding("TagManager.Sorting");
            binding.Converter = TagSortingToBool;
            binding.ConverterParameter = converterParameter;
            childMenuitem.SetBinding(System.Windows.Controls.MenuItem.IsCheckedProperty, binding);
        }

        private void ClearResultSearchingByTag()
        {
            var activeViewModel = MainWindowViewModel.ActiveDocumentViewModel;
            activeViewModel.BookCabinet.ClearSearchResult();
        }

        private void SearchByTag(IEnumerable<TagCountViewModel> items)
        {
            var activeViewModel = MainWindowViewModel.ActiveDocumentViewModel;
            activeViewModel.BookCabinet.ClearSearchResult();
            foreach (var item in items)
            {
                item.IsSearchingKey = true;
            }
            TagManager.ShowBySelectedItems(items.Select(itc => itc.Tag));
            activeViewModel.ResetScrollOffset();
        }

        public bool SortingSelected(string name)
        {
            return Querying.SortingSelected(TagManager.Sorting, name);
        }
    }
}
