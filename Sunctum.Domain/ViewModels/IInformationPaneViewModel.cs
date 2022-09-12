using System;
using System.Collections.Generic;

namespace Sunctum.Domain.ViewModels
{
    public interface IInformationPaneViewModel
    {
        Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }
        List<TagViewModel> TagListBoxSelectedItems { get; set; }
    }
}