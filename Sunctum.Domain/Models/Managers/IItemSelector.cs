

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Models.Managers
{
    public interface IItemSelector<T>
    {
        List<T> SelectedItems { get; set; }

        void ShowBySelectedItems(IMainWindowViewModel mainWindowViewModel, IEnumerable<T> searchItems);
    }
}
