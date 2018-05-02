

using Sunctum.Domain.ViewModels;
using System.Collections.Generic;

namespace Sunctum.Domain.Logic.AuthorSorting
{
    public interface IAuthorSorting
    {
        IEnumerable<AuthorCountViewModel> Sort(IEnumerable<AuthorCountViewModel> loadedSource);
    }
}
