

using System.Collections.Generic;

namespace Sunctum.Domain.Models.Managers
{
    public interface IItemSelector<T>
    {
        List<T> SelectedItems { get; set; }

        void ShowBySelectedItems(ILibraryManager library, IEnumerable<T> searchItems);
    }
}
