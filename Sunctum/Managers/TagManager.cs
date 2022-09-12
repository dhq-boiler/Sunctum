

using Homura.Core;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Sunctum.Core.Notifications;
using Sunctum.Domail.Util;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.ImageTagCountSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace Sunctum.Managers
{
    public class TagManager : BindableBase, ITagManager, IObserver<ActiveTabChanged>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<TagViewModel> _Tags;
        private ObservableCollection<ImageTagViewModel> _Chains;
        private ObservableCollection<TagCountViewModel> _TagCount;
        private List<EntryViewModel> _SelectedEntries;
        private List<TagViewModel> _SelectedEntityTags;
        private List<TagViewModel> _SelectedItems;
        private bool _OrderAscending;
        private ObservableCollection<TagCountViewModel> _SearchedImageTags;
        private IImageTagCountSorting _ImageTagCountSorting;

        public IProgressManager ProgressManager { get; set; } = new ProgressManager();

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
            LoadBookTag();
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
                        if (removetag.Tag is null)
                        {
                            removetag.Tag = TagFacade.FindBy(removetag.TagID);
                        }
                        if (TagCount.Where(tc => tc.Tag.Name == removetag.Tag.Name).Count() == 1)
                        {
                            var tagCount = TagCount.Single(tc => tc.Tag.Name == removetag.Tag.Name);
                            tagCount.Count--;
                        }
                    }
                    break;
            }
        }

        #region コマンド

        public ICommand RemoveTagFromEntriesCommand { get; set; }

        public ICommand SortByNameAscCommand { get; set; }

        public ICommand SortByNameDescCommand { get; set; }

        public ICommand SortByCountAscCommand { get; set; }

        public ICommand SortByCountDescCommand { get; set; }

        #endregion //コマンド

        #region コマンド登録

        private void RegisterCommands()
        {
            RemoveTagFromEntriesCommand = new DelegateCommand<object>(async (p) =>
            {
                await RemoveImageTag(p as string);
            });
            SortByNameAscCommand = new DelegateCommand(() =>
            {
                Sorting = ImageTagCountSorting.ByNameAsc;
            });
            SortByNameDescCommand = new DelegateCommand(() =>
            {
                Sorting = ImageTagCountSorting.ByNameDesc;
            });
            SortByCountAscCommand = new DelegateCommand(() =>
            {
                Sorting = ImageTagCountSorting.ByCountAsc;
            });
            SortByCountDescCommand = new DelegateCommand(() =>
            {
                Sorting = ImageTagCountSorting.ByCountDesc;
            });
        }

        #endregion //コマンド登録

        #region プロパティ

        [Dependency]
        public ITaskManager TaskManager { get; set; }

        [Dependency]
        public IImageTagAdding ImageTagAddingService { get; set; }

        [Dependency]
        public IImageTagRemoving ImageTagRemovingService { get; set; }

        [Dependency]
        public ITagRemoving TagRemovingService { get; set; }

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

        public ObservableCollection<BookTagViewModel> BookTagChains
        {
            [DebuggerStepThrough]
            get
            { return _BookTagChains; }
            set { SetProperty(ref _BookTagChains, value); }
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

        public IImageTagCountSorting Sorting
        {
            [DebuggerStepThrough]
            get
            { return _ImageTagCountSorting; }
            set
            {
                SetProperty(ref _ImageTagCountSorting, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
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
                var newCollection = Sorting.Sort(DisplayableImageTagCountSource).ToArray();
                return new ObservableCollection<TagCountViewModel>(newCollection);
            }
        }

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

        private void LoadBookTag()
        {
            var sw = new Stopwatch();
            s_logger.Info("Loading BookTag list...");
            sw.Start();
            try
            {
                BookTagChains = new ObservableCollection<BookTagViewModel>(BookTagFacade.FindAll());
            }
            finally
            {
                s_logger.Info($"Completed to load BookTag list. {sw.ElapsedMilliseconds}ms");
            }
        }

        private void Filter(ObservableCollection<BookViewModel> books)
        {
            var timeKeeper = new TimeKeeper();

            ProgressManager.UpdateProgress(0, OnStage.Count, timeKeeper);

            var bookIds = new HashSet<Guid>(books.Select(b => b.ID));
            var filteredBookTags = new HashSet<Guid>(BookTagChains.Where(bt => bookIds.Contains(bt.BookID)).Select(bt => bt.TagID));

            var i = 0;
            foreach (var tagCount in OnStage.Reverse())
            {
                tagCount.IsVisible = filteredBookTags.Contains(tagCount.Tag.ID);

                ++i;
                ProgressManager.UpdateProgress(i, TagCount.Count, timeKeeper);
            }
            ProgressManager.Complete();
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
                    var tags = (from c in Chains
                                where c.ImageID == images.First().ID
                                join t in Tags on c.TagID equals t.ID
                                select t).ToList();
                    temp.AddRange(tags);
                }

                foreach (var image in images)
                {
                    var tags = (from c in Chains
                                where c.ImageID == image.ID
                                join t in Tags on c.TagID equals t.ID
                                select t).ToList();
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

        public void ShowBySelectedItems(IMainWindowViewModel mainWindowViewModel)
        {
            var activeViewModel = mainWindowViewModel.ActiveDocumentViewModel;

            var images = ImageFacade.FindAll();
            var pages = PageFacade.FindAll();
            var books = activeViewModel.BookCabinet.BookSource.Select(b => b);

            books = (from bk in books
                     join pg in pages on bk.ID equals pg.BookID
                     join img in images on pg.ImageID equals img.ID
                     join ic in Chains on img.ID equals ic.ImageID
                     join tg in SelectedItems on ic.TagID equals tg.ID
                     select bk).Distinct();

            activeViewModel.BookCabinet.SearchText = $"{ToSearchText(SelectedItems)}";
            activeViewModel.BookCabinet.SearchedBooks = new ReactiveCollection<BookViewModel>();
            activeViewModel.BookCabinet.SearchedBooks.AddRange(books.ToList());
        }

        private string ToSearchText(List<TagViewModel> selectedItems)
        {
            var sb = new StringBuilder();
            foreach (var item in selectedItems)
            {
                sb.Append(item.Name);
                if (selectedItems.Last() != item)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public void ShowBySelectedItems(IMainWindowViewModel mainWindowViewModel, IEnumerable<TagViewModel> searchItems)
        {
            SelectedItems = searchItems.ToList();

            ShowBySelectedItems(mainWindowViewModel);
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

            RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => OnStage));
        }

        private object _lock_object = new object();
        private CancellationTokenSource _tokenSource;
        private ObservableCollection<BookTagViewModel> _BookTagChains;

        public void OnNext(ActiveTabChanged value)
        {
            lock (_lock_object)
            {
                if (_tokenSource != null)
                {
                    _tokenSource.Cancel();
                }
            }
            _tokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                Filter(value.BookStorage.BookSource);
                lock (_lock_object)
                {
                    _tokenSource = null;
                }
            }, _tokenSource.Token);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
