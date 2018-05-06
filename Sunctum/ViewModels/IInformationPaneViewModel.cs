

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.ViewModels
{
    public interface IInformationPaneViewModel
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }
        List<TagViewModel> TagListBoxSelectedItems { get; set; }
        ITagManager TagManager { get; set; }
    }
}