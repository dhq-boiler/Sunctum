

using Sunctum.Domain.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models.Managers
{
    public interface IAuthorManager : IItemSelector<AuthorViewModel>
    {
        void Load();

        Task LoadAsync();

        ObservableCollection<AuthorViewModel> Authors { get; }

        ObservableCollection<AuthorCountViewModel> AuthorCount { get; }

        bool EnableOrderByName { get; set; }

        void SwitchOrdering();

        void ObserveAuthorCount();

        void ClearSearchResult();

        void LoadAuthorCount();

        void LoadedBooks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        bool AnySearchingKeys();
    }
}
