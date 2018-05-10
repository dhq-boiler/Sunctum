

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Sunctum.ViewModels
{
    public interface IAuthorPaneViewModel
    {
        ObservableCollection<Control> AuthorContextMenuItems { get; set; }
        List<AuthorCountViewModel> AuthorListBoxSelectedItems { get; set; }
        IAuthorManager AuthorManager { get; set; }
        ILibrary LibraryManager { get; set; }
        IMainWindowViewModel MainWindowViewModel { get; set; }

        void BuildContextMenus_Authors();
        void ClearSelectedItems();
    }
}