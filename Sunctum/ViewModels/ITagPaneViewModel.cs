

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public interface ITagPaneViewModel
    {
        ICommand ClearResultSearchingByTagCommand { get; set; }
        IMainWindowViewModel MainWindowViewModel { get; set; }
        ObservableCollection<Control> TagContextMenuItems { get; set; }
        List<TagCountViewModel> TagListBoxSelectedItems { get; set; }

        void BuildContextMenus_Tags();
        void ClearSelectedItems();
    }
}