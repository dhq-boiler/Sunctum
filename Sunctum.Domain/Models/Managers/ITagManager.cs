

using Sunctum.Domain.Logic.ImageTagCountSorting;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sunctum.Domain.Models.Managers
{
    public interface ITagManager : IItemSelector<TagViewModel>
    {
        ObservableCollection<TagViewModel> Tags { get; }

        ObservableCollection<ImageTagViewModel> Chains { get; }

        ObservableCollection<TagCountViewModel> TagCount { get; }

        ObservableCollection<BookTagViewModel> BookTagChains { get; }

        List<TagViewModel> SelectedEntityTags { get; set; }

        List<EntryViewModel> SelectedEntries { get; }

        Task AddTagTo(IEnumerable<EntryViewModel> entries, string tagName);

        Task AddImageTagToSelectedObject(string tagName);

        void Unselect(IEnumerable<EntryViewModel> enumerable);

        void ClearSelectedEntries();

        void AddToSelectedEntry(EntryViewModel add);

        void AddToSelectedEntries(IEnumerable<EntryViewModel> enumerable);

        void ObserveSelectedEntityTags();

        void RemoveByImage(ImageViewModel image);

        Task RemoveImageTag(string tagName);

        Task RemoveImageTag(IEnumerable<string> tagNames);

        void Load();

        Task LoadAsync();

        List<TagViewModel> GetCommonTags();

        void ObserveTagCount();

        Task RemoveTag(IEnumerable<string> tagNames);

        void ClearSearchResult();

        bool IsSearching();

        ICommand SortByNameAscCommand { get; set; }

        ICommand SortByNameDescCommand { get; set; }

        ICommand SortByCountAscCommand { get; set; }

        ICommand SortByCountDescCommand { get; set; }

        IImageTagCountSorting Sorting { get; set; }
    }
}
