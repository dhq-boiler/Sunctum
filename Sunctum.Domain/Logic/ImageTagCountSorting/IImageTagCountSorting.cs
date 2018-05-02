

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.ImageTagCountSorting
{
    public interface IImageTagCountSorting
    {
        IEnumerable<TagCountViewModel> Sort(IEnumerable<TagCountViewModel> loadedSource);
    }
}
