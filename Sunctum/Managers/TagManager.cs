

using Ninject;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.ImageTagCountSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sunctum.Managers
{
    public class TagManager : BindableBase, ITagManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<TagViewModel> _Tags;
        private ObservableCollection<ImageTagViewModel> _Chains;
        private ObservableCollection<TagCountViewModel> _TagCount;
        private List<EntryViewModel> _SelectedEntries;
        private List<TagViewModel> _SelectedEntityTags;
        private List<TagViewModel> _SelectedItems;
        private bool _OrderAscending;
        private bool _EnableOrderByName;
        private ObservableCollection<TagCountViewModel> _SearchedImageTags;
        private IImageTagCountSorting _ImageTagCountSorting;

        #region コマンド

        public ICommand RemoveTagFromEntriesCommand { get; set; }

        #endregion //コマンド

        public TagManager()
        {
            RegisterCommands();
            Sorting = ImageTagCountSorting.ByNameAsc;

            Tags = new ObservableCollection<TagViewModel>();
            Tags.CollectionChanged += Tags_CollectionChanged;

            Chains = new ObservableCollection<ImageTagViewModel>();
            Chains.CollectionChanged += Chains_CollectionChanged;

            TagCount = new ObservableCollection<TagCountViewModel>();
            SelectedEntries = new List<EntryViewModel>();
            ObserveSelectedEntityTags();
        }

        public async Task LoadAsync()
        {
            await Task.Run(() => Load());
        }

        public void Load()
        {
            if (Tags != null)
            {
                Tags.CollectionChanged -= Tags_CollectionChanged;
            }

            if (Chains != null)
            {
                Chains.CollectionChanged -= Chains_CollectionChanged;
            }

            LoadTag();
            LoadImageTag();
            TagCount = new ObservableCollection<TagCountViewModel>(GenerateTagCount());
            SelectedEntries = new List<EntryViewModel>();
            ObserveSelectedEntityTags();
        }

        private void Tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObserveTagCount();
        }

        private void Chains_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newtag in e.NewItems.Cast<ImageTagViewModel>())
                    {
                        if (TagCount.Where(tc => tc.Tag.Name == newtag.Tag.Name).Count() == 1)
                        {
                            var tagCount = TagCount.Single(tc => tc.Tag.Name == newtag.Tag.Name);
                            tagCount.Count++;
                        }
                        else if (TagCount.Where(tc => tc.Tag.Name == newtag.Tag.Name).Count() == 0)
                        {
                            TagCount.Add(new TagCountViewModel(newtag.Tag, 1));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var removetag in e.OldItems.Cast<ImageTagViewModel>())
                    {
                        if (TagCount.Where(tc => tc.Tag.Name == removetag.Tag.Name).Count() == 1)
                        {
                            var tagCount = TagCount.Single(tc => tc.Tag.Name == removetag.Tag.Name);
                            tagCount.Count--;
                        }
                    }
                    break;
            }
        }

        private void RegisterCommands()
        {
            RemoveTagFromEntriesCommand = new DelegateCommand<object>(async (p) =>
            {
                await RemoveImageTag(p as string);
            });
        }

        #region プロパティ

        [Inject]
        public ITaskManager TaskManager { get; set; }

        public ObservableCollection<TagViewModel> Tags
        {
            [DebuggerStepThrough]
            get
            { return _Tags; }
            set
            {
                value.CollectionChanged += (s, e) => ObserveTagCount();
                SetProperty(ref _Tags, value);
            }
        }

        public ObservableCollection<ImageTagViewModel> Chains
        {
            [DebuggerStepThrough]
            get
            { return _Chains; }
            set { SetProperty(ref _Chains, value); }
        }

        public List<EntryViewModel> SelectedEntries
        {
            [DebuggerStepThrough]
            get
            { return _SelectedEntries; }
            set { SetProperty(ref _SelectedEntries, value); }
        }

        public List<TagViewModel> SelectedEntityTags
        {
            [DebuggerStepThrough]
            get
            { return _SelectedEntityTags; }
            set { SetProperty(ref _SelectedEntityTags, value); }
        }

        public ObservableCollection<TagCountViewModel> TagCount
        {
            get { return _TagCount; }
            set
            {
                SetProperty(ref _TagCount, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public List<TagViewModel> SelectedItems
        {
            [DebuggerStepThrough]
            get
            { return _SelectedItems; }
            set { SetProperty(ref _SelectedItems, value); }
        }

        public bool EnableOrderByName
        {
            get { return _EnableOrderByName; }
            set
            {
                SetProperty(ref _EnableOrderByName, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        public IImageTagCountSorting Sorting
        {
            [DebuggerStepThrough]
            get
            { return _ImageTagCountSorting; }
            set
            {
                SetProperty(ref _ImageTagCountSorting, value);
            }
        }

        public void ObserveSelectedEntityTags()
        {
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => SelectedEntityTags));
        }

        public void ObserveTagCount()
        {
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => TagCount));
        }

        public ObservableCollection<TagCountViewModel> SearchedImageTagCounts
        {
            [DebuggerStepThrough]
            get
            { return _SearchedImageTags; }
            set
            {
                SetProperty(ref _SearchedImageTags, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
            }
        }

        private ObservableCollection<TagCountViewModel> DisplayableImageTagCountSource
        {
            [DebuggerStepThrough]
            get
            {
                if (SearchedImageTagCounts != null)
                {
                    return SearchedImageTagCounts;
                }
                else
                {
                    return TagCount;
                }
            }
        }

        public ObservableCollection<TagCountViewModel> OnStage
        {
            get
            {
                if (EnableOrderByName)
                {
                    if (_OrderAscending)
                    {
                        Sorting = ImageTagCountSorting.ByNameAsc;
                    }
                    else
                    {
                        Sorting = ImageTagCountSorting.ByNameDesc;
                    }
                }
                else
                {
                    if (_OrderAscending)
                    {
                        Sorting = ImageTagCountSorting.ByCountAsc;
                    }
                    else
                    {
                        Sorting = ImageTagCountSorting.ByCountDesc;
                    }
                }

                var newCollection = Sorting.Sort(DisplayableImageTagCountSource).ToArray();
                return new ObservableCollection<TagCountViewModel>(newCollection);
            }
        }

        [Inject]
        public IImageTagAdding ImageTagAddingService { get; set; }

        [Inject]
        public IImageTagRemoving ImageTagRemovingService { get; set; }

        [Inject]
        public ITagRemoving TagRemovingService { get; set; }

        #endregion //プロパティ

        private void LoadTag()
        {
            Stopwatch sw = new Stopwatch();
            s_logger.Info("Loading Tag list...");
            sw.Start();
            try
            {
                Tags = new ObservableCollection<TagViewModel>(TagFacade.FindAll());
                Tags.CollectionChanged += Tags_CollectionChanged;
            }
            finally
            {
                s_logger.Info($"Completed to load Tag list. {sw.ElapsedMilliseconds}ms");
            }
        }

        private void LoadImageTag()
        {
            Stopwatch sw = new Stopwatch();
            s_logger.Info("Loading ImageTag list...");
            sw.Start();
            try
            {
                Chains = new ObservableCollection<ImageTagViewModel>(ImageTagFacade.FindAll());
                Chains.CollectionChanged += Chains_CollectionChanged;
            }
            finally
            {
                s_logger.Info($"Completed to load ImageTag list. {sw.ElapsedMilliseconds}ms");
            }
        }

        private IEnumerable<TagCountViewModel> GenerateTagCount()
        {
            Stopwatch sw = new Stopwatch();
            s_logger.Info($"Loading TagCount list...");
            sw.Start();
            try
            {
                return ImageTagFacade.FindAllAsCount();
            }
            finally
            {
                sw.Stop();
                s_logger.Info($"Completed to load TagCount list. {sw.ElapsedMilliseconds}ms");
            }
        }

        public List<TagViewModel> GetCommonTags()
        {
            if (SelectedEntries == null || SelectedEntries.Count() == 0) return new List<TagViewModel>();

            var images = GetAllImages(SelectedEntries);
            if (images.Count() == 0) return new List<TagViewModel>();

            var firstEntry = images.First();
            IEnumerable<TagViewModel> temp = Chains.Where(a => a.ImageID == firstEntry.ID).Select(a => a.Tag).ToList();

            foreach (var image in images)
            {
                IEnumerable<TagViewModel> tags = Chains.Where(a => a.ImageID == image.ID).Select(a => a.Tag).ToList();
                temp = temp.Intersect(tags).ToList();
            }

            return temp.ToList();
        }

        private IEnumerable<ImageViewModel> GetAllImages(IEnumerable<EntryViewModel> entries)
        {
            return ImageFacade.GetAllImages(entries);
        }

        public async Task AddTagTo(IEnumerable<EntryViewModel> entries, string tagName)
        {
            Contract.Assert(entries != null);
            Contract.Assert(entries.Count() > 0);
            Contract.Assert(tagName != null);
            Contract.Assert(tagName.Trim().Count() > 0);

            ImageTagAddingService.Entries = entries;
            ImageTagAddingService.TagName = tagName;
            await TaskManager.Enqueue(ImageTagAddingService.GetTaskSequence());
        }

        public async Task AddImageTagToSelectedObject(string tagName)
        {
            if (tagName == null || tagName.Count() == 0)
            {
                throw new ArgumentException("tagName == null || tagName.Count() == 0");
            }

            if (SelectedEntries != null)
            {
                ImageTagAddingService.Entries = SelectedEntries;
                ImageTagAddingService.TagName = tagName;
                await TaskManager.Enqueue(ImageTagAddingService.GetTaskSequence());
            }
        }

        public async Task RemoveImageTag(string tagName)
        {
            if (tagName == null || tagName.Count() == 0)
            {
                throw new ArgumentException("tagName == null || tagName.Count() == 0");
            }

            if (SelectedEntries != null)
            {
                ImageTagRemovingService.Entries = SelectedEntries;
                ImageTagRemovingService.TagNames = new string[] { tagName };
                await TaskManager.Enqueue(ImageTagRemovingService.GetTaskSequence());
            }
        }

        public void AddToSelectedEntry(EntryViewModel add)
        {
            RecaclulateCommonTags(add);
            SelectedEntries.Add(add);
        }

        public void AddToSelectedEntries(IEnumerable<EntryViewModel> enumerable)
        {
            RecaclulateCommonTags(enumerable);
            SelectedEntries.AddRange(enumerable);
        }

        private void RecaclulateCommonTags(EntryViewModel add)
        {
            var images = GetAllImages(new EntryViewModel[] { add });
            try
            {
                if (SelectedEntries.Count == 0 && images.Count() > 0)
                {
                    IEnumerable<TagViewModel> tags = Chains.Where(a => a.ImageID == images.First().ID).Select(a => a.Tag).ToList();
                    SelectedEntityTags.AddRange(tags);
                }

                foreach (var image in images)
                {
                    IEnumerable<TagViewModel> tags = Chains.Where(a => a.ImageID == image.ID).Select(a => a.Tag).ToList();
                    SelectedEntityTags = SelectedEntityTags.Intersect(tags).ToList();
                }
            }
            catch (InvalidOperationException e)
            {
                s_logger.Warn(e);
            }
        }

        private void RecaclulateCommonTags(IEnumerable<EntryViewModel> add)
        {
            var images = GetAllImages(add);
            try
            {
                List<TagViewModel> temp = new List<TagViewModel>(SelectedEntityTags);

                if (SelectedEntries.Count == 0 && images.Count() > 0)
                {
                    IEnumerable<TagViewModel> tags = Chains.Where(a => a.ImageID == images.First().ID).Select(a => a.Tag).ToList();
                    temp.AddRange(tags);
                }

                foreach (var image in images)
                {
                    IEnumerable<TagViewModel> tags = Chains.Where(a => a.ImageID == image.ID).Select(a => a.Tag).ToList();
                    temp = temp.Intersect(tags).ToList();
                }
                SelectedEntityTags = temp;
            }
            catch (InvalidOperationException e)
            {
                s_logger.Warn(e);
            }
        }

        public void Unselect(IEnumerable<EntryViewModel> enumerable)
        {
            foreach (var entry in enumerable)
            {
                SelectedEntries.Remove(entry);
            }
            SelectedEntityTags = GetCommonTags();
        }

        public void RemoveByImage(ImageViewModel image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image == null");
            }

            ImageTagFacade.DeleteWhereIDIs(image.ID);

            var removeList = Chains.Where(it => it.ImageID == image.ID).ToList();
            foreach (var remove in removeList)
            {
                Chains.Remove(remove);
            }
        }

        public async Task RemoveImageTag(IEnumerable<string> tagNames)
        {
            if (tagNames == null || tagNames.Count() == 0)
            {
                throw new ArgumentException("tagName == null || tagName.Count() == 0");
            }

            if (SelectedEntries != null)
            {
                ImageTagRemovingService.Entries = SelectedEntries;
                ImageTagRemovingService.TagNames = tagNames;
                await TaskManager.Enqueue(ImageTagRemovingService.GetTaskSequence());
            }
        }

        public void ClearSelectedEntries()
        {
            SelectedEntries.Clear();
        }

        public async Task RemoveTag(IEnumerable<string> tagNames)
        {
            if (tagNames == null || tagNames.Count() == 0)
            {
                throw new ArgumentException("tagNames == null || tagNames.Count() == 0");
            }

            if (SelectedEntries != null)
            {
                TagRemovingService.TagNames = tagNames;
                await TaskManager.Enqueue(TagRemovingService.GetTaskSequence());
            }
        }

        public void ShowBySelectedItems(ILibraryManager library)
        {
            Contract.Requires(library != null);
            Contract.Requires(library.TagMng != null);
            Contract.Requires(library.TagMng.SelectedItems != null);

            var images = ImageFacade.FindAll();
            var pages = PageFacade.FindAll();
            var books = library.LoadedBooks.Select(b => b);

            books = (from bk in books
                     join pg in pages on bk.ID equals pg.BookID
                     join img in images on pg.ImageID equals img.ID
                     join ic in library.TagMng.Chains on img.ID equals ic.ImageID
                     join tg in library.TagMng.SelectedItems on ic.TagID equals tg.ID
                     select bk).Distinct();

            library.SearchedBooks = new ObservableCollection<BookViewModel>(books.ToList());
        }

        public void ShowBySelectedItems(ILibraryManager library, IEnumerable<TagViewModel> searchItems)
        {
            SelectedItems = searchItems.ToList();

            ShowBySelectedItems(library);
        }

        public bool IsSearching()
        {
            if (_TagCount == null) return false;

            return _TagCount.Any(tc => tc.IsSearchingKey);
        }

        public void ClearSearchResult()
        {
            if (_TagCount == null) return;

            foreach (var item in _TagCount)
            {
                item.IsSearchingKey = false;
            }
        }

        public void SwitchOrdering()
        {
            _OrderAscending = !_OrderAscending;

            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OrderText));
            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        public string OrderText
        {
            get
            {
                if (_OrderAscending)
                    return "↑";
                else
                    return "↓";
            }
        }
    }
}
