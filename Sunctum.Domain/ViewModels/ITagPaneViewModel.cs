using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.Domain.ViewModels
{
    public interface ITagPaneViewModel
    {
        ICommand ClearResultSearchingByTagCommand { get; set; }
        Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }
        ObservableCollection<Control> TagContextMenuItems { get; set; }
        List<TagCountViewModel> TagListBoxSelectedItems { get; set; }

        void BuildContextMenus_Tags();
        void ClearSelectedItems();
        bool SortingSelected(string name);
    }
}