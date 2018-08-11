

using Sunctum.Domain.Logic.AuthorSorting;
using Sunctum.Domain.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sunctum.Domain.Models.Managers
{
    public interface IAuthorManager : IItemSelector<AuthorViewModel>
    {
        void Load();

        Task LoadAsync();

        ObservableCollection<AuthorViewModel> Authors { get; }

        ObservableCollection<AuthorCountViewModel> AuthorCount { get; }

        void ObserveAuthorCount();

        void ClearSearchResult();

        void LoadAuthorCount();

        void LoadedBooks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        bool AnySearchingKeys();

        IAuthorSorting Sorting { get; set; }

        ICommand SortByNameAscCommand { get; }

        ICommand SortByNameDescCommand { get; }

        ICommand SortByCountAscCommand { get; }

        ICommand SortByCountDescCommand { get; }
    }
}
