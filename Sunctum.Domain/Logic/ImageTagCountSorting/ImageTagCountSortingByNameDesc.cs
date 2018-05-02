

using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Logic.ImageTagCountSorting
{
    public class ImageTagCountSortingByNameDesc : IImageTagCountSorting
    {
        public IEnumerable<TagCountViewModel> Sort(IEnumerable<TagCountViewModel> loadedSource)
        {
            return loadedSource.OrderByDescending(itc => itc.Tag.Name, new NaturalStringComparer());
        }
    }
}