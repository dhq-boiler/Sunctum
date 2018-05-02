

using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Domain.Logic.AuthorSorting
{
    public class AuthorSortingByNameAsc : IAuthorSorting
    {
        public IEnumerable<AuthorCountViewModel> Sort(IEnumerable<AuthorCountViewModel> loadedSource)
        {
            Contract.Requires(loadedSource != null);
            return loadedSource.OrderBy(ac => ac.Author.Name, new NaturalStringComparer());
        }
    }
}