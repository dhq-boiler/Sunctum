

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class ImageTagAdding : AsyncTaskMakerBase, IImageTagAdding
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private TagViewModel _tag;

        private IEnumerable<PageViewModel> _pages;

        private List<ImageViewModel> _images;

        [Inject]
        public ITagManager TagManager { get; set; }

        public IEnumerable<EntryViewModel> Entries { get; set; }

        public string TagName { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start ImageTagAdding"));
            sequence.Add(() => s_logger.Info($"      TagName : {TagName}"));
            sequence.Add(() => s_logger.Info($"      Entries : {Entries.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(new Task(() =>
            {
                if (TagFacade.Exists(TagName))
                {
                    _tag = TagFacade.FindByTagName(TagName);
                }
                else
                {
                    _tag = new TagViewModel(Guid.NewGuid(), TagName);
                    TagFacade.Insert(_tag);
                }
            }));

            sequence.Add(new Task(() =>
            {
                _images = new List<ImageViewModel>();
            }));

            foreach (var entry in Entries)
            {
                var book = entry as BookViewModel;
                if (book != null)
                {
                    _pages = PageFacade.FindByBookId(book.ID);

                    for (int i = 0; i < _pages.Count(); ++i)
                    {
                        var p = _pages.ElementAt(i);
                        sequence.Add(new Task(() =>
                        {
                            GetPropertyIfImageIsNull(ref p);
                            _images.Add(p.Image);
                        }));
                    }
                    sequence.Add(new Task(() =>
                    {
                        var newEntity = new BookTagViewModel(book, _tag);
                        if (!BookTagFacade.Exists(newEntity))
                        {
                            BookTagFacade.Insert(newEntity);
                        }
                    }));
                    continue;
                }

                var page = entry as PageViewModel;
                if (page != null)
                {
                    sequence.Add(new Task(() =>
                    {
                        GetPropertyIfImageIsNull(ref page);
                        _images.Add(page.Image);
                    }));

                    sequence.Add(new Task(() =>
                    {
                        var newEntity = new BookTagViewModel(page.BookID, _tag.ID);
                        if (!BookTagFacade.Exists(newEntity))
                        {
                            BookTagFacade.Insert(newEntity);
                        }
                    }));
                    continue;
                }

                var image = entry as ImageViewModel;
                if (image != null)
                {
                    sequence.Add(new Task(() =>
                    {
                        _images.Add(image);
                    }));

                    sequence.Add(new Task(() =>
                    {
                        var tempPage = PageFacade.FindByImageId(image.ID);
                        var newEntity = new BookTagViewModel(tempPage.BookID, _tag.ID);
                        if (!BookTagFacade.Exists(newEntity))
                        {
                            BookTagFacade.Insert(newEntity);
                        }
                    }));
                    continue;
                }
            }

            sequence.Add(new Task(() =>
            {
                ImageTagFacade.BatchInsert(_tag, _images);
            }));

            sequence.Add(new Task(() =>
            {
                var chains = from x in _images
                             where TagManager.Chains.Count(c => c.ImageID == x.ID && c.TagID == _tag.ID) == 0
                             select new ImageTagViewModel(x.ID, _tag);

                foreach (var chain in chains)
                {
                    TagManager.Chains.Add(chain);
                }
            }));

            sequence.Add(new Task(() =>
            {
                if (!TagManager.Tags.Contains(_tag))
                {
                    TagManager.Tags.Add(_tag);
                }
            }));

            sequence.Add(new Task(() => TagManager.SelectedEntityTags = TagManager.GetCommonTags()));
            sequence.Add(new Task(() => TagManager.ObserveSelectedEntityTags()));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ImageTagAdding"));
        }

        public static List<Task> GenerateAddImageTagTasks(ITagManager tagManager, IEnumerable<EntryViewModel> entries, string tagName)
        {
            List<Task> tasks = new List<Task>();

            var ita = new ImageTagAdding();

            tasks.Add(new Task(() =>
            {
                if (TagFacade.Exists(tagName))
                {
                    ita._tag = TagFacade.FindByTagName(tagName);
                }
                else
                {
                    ita._tag = new TagViewModel(Guid.NewGuid(), tagName);
                    TagFacade.Insert(ita._tag);
                }
            }));

            tasks.Add(new Task(() =>
            {
                ita._images = new List<ImageViewModel>();
            }));

            foreach (var entry in entries)
            {
                var book = entry as BookViewModel;
                if (book != null)
                {
                    ita._pages = PageFacade.FindByBookId(book.ID);

                    for (int i = 0; i < ita._pages.Count(); ++i)
                    {
                        var p = ita._pages.ElementAt(i);
                        tasks.Add(new Task(() =>
                        {
                            GetPropertyIfImageIsNull(ref p);
                            ita._images.Add(p.Image);
                        }));
                    }
                    continue;
                }

                var page = entry as PageViewModel;
                if (page != null)
                {
                    tasks.Add(new Task(() =>
                    {
                        GetPropertyIfImageIsNull(ref page);
                        ita._images.Add(page.Image);
                    }));
                    continue;
                }

                var image = entry as ImageViewModel;
                if (image != null)
                {
                    tasks.Add(new Task(() =>
                    {
                        ita._images.Add(image);
                    }));
                    continue;
                }
            }

            tasks.Add(new Task(() =>
            {
                ImageTagFacade.BatchInsert(ita._tag, ita._images);
            }));

            tasks.Add(new Task(() =>
            {
                var chains = from x in ita._images
                             where tagManager.Chains.Count(c => c.ImageID == x.ID && c.TagID == ita._tag.ID) == 0
                             select new ImageTagViewModel(x.ID, ita._tag);

                foreach (var chain in chains)
                {
                    tagManager.Chains.Add(chain);
                }
            }));

            tasks.Add(new Task(() =>
            {
                if (!tagManager.Tags.Contains(ita._tag))
                {
                    tagManager.Tags.Add(ita._tag);
                }
            }));

            tasks.Add(new Task(() => tagManager.SelectedEntityTags = tagManager.GetCommonTags()));
            tasks.Add(new Task(() => tagManager.ObserveSelectedEntityTags()));

            return tasks;
        }

        private static void GetPropertyIfImageIsNull(ref PageViewModel p)
        {
            if (p.Image == null)
            {
                PageFacade.GetProperty(ref p);
            }
        }
    }
}
