

using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;

namespace Sunctum.ViewModels
{
    public interface IInformationPaneViewModel
    {
        Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }
        List<TagViewModel> TagListBoxSelectedItems { get; set; }
    }
}