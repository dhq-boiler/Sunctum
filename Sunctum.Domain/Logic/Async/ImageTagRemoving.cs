using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class ImageTagRemoving : AsyncTaskMakerBase, IImageTagRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<EntryViewModel> Entries { get; set; }

        [Dependency]
        public Lazy<ITagManager> TagManager { get; set; }

        public IEnumerable<string> TagNames { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start ImageTagRemoving"));
            sequence.Add(() => s_logger.Info($"      Entries : {Entries.ArrayToString()}"));
            sequence.Add(() => s_logger.Info($"      TagNames : {TagNames.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var tagName in TagNames)
            {
                foreach (var entry in Entries)
                {
                    var book = entry as BookViewModel;
                    if (book != null)
                    {
                        var pages = PageFacade.FindByBookId(book.ID);
                        for (int i = 0; i < pages.Count(); ++i)
                        {
                            var p = pages.ElementAt(i);
                            sequence.Add(new Task(() =>
                            {
                                GetPropertyIfImageIsNull(ref p);
                            }));
                            sequence.Add(new Task(() =>
                            {
                                Remove.ImageTagRemoving.Remove(TagManager.Value, p.Image, tagName);
                            }));
                        }

                        sequence.Add(new Task(() =>
                        {
                            var tag = TagFacade.FindByTagName(tagName);
                            var deleteEntity = new BookTagViewModel(book.ID, tag.ID);
                            if (BookTagFacade.Exists(deleteEntity))
                            {
                                BookTagFacade.Delete(deleteEntity);
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
                        }));
                        sequence.Add(new Task(() =>
                        {
                            Remove.ImageTagRemoving.Remove(TagManager.Value, page.Image, tagName);
                        }));

                        sequence.Add(new Task(() =>
                        {
                            var tag = TagFacade.FindByTagName(tagName);
                            var deleteEntity = new BookTagViewModel(book.ID, tag.ID);

                            var pages = PageFacade.FindByBookId(page.BookID);
                            foreach (var p in pages)
                            {
                                if (p.ID.Equals(page.ID))
                                {
                                    continue;
                                }

                                var imageTags = ImageTagFacade.FindByImageId(p.ImageID);
                                foreach (var imageTag in imageTags)
                                {
                                    if (imageTag.TagID.Equals(deleteEntity.TagID))
                                    {
                                        return;
                                    }
                                }
                            }

                            if (BookTagFacade.Exists(deleteEntity))
                            {
                                BookTagFacade.Delete(deleteEntity);
                            }
                        }));
                        continue;
                    }

                    var image = entry as ImageViewModel;
                    if (image != null)
                    {
                        sequence.Add(new Task(() =>
                        {
                            Remove.ImageTagRemoving.Remove(TagManager.Value, page.Image, tagName);
                        }));

                        sequence.Add(new Task(() =>
                        {
                            var tag = TagFacade.FindByTagName(tagName);
                            var deleteEntity = new BookTagViewModel(book.ID, tag.ID);

                            var parentPage = PageFacade.FindByImageId(image.ID);
                            var pages = PageFacade.FindByBookId(parentPage.BookID);
                            foreach (var p in pages)
                            {
                                if (p.ID.Equals(parentPage.ID))
                                {
                                    continue;
                                }

                                var imageTags = ImageTagFacade.FindByImageId(p.ImageID);
                                foreach (var imageTag in imageTags)
                                {
                                    if (imageTag.TagID.Equals(deleteEntity.TagID))
                                    {
                                        return;
                                    }
                                }
                            }

                            if (BookTagFacade.Exists(deleteEntity))
                            {
                                BookTagFacade.Delete(deleteEntity);
                            }
                        }));
                        continue;
                    }
                }

                sequence.Add(new Task(() => TagManager.Value.SelectedEntityTags = TagManager.Value.GetCommonTags()));
                sequence.Add(new Task(() => TagManager.Value.ObserveSelectedEntityTags()));
                sequence.Add(new Task(() => TagManager.Value.ObserveTagCount()));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ImageTagRemoving"));
        }

        public static List<Task> GenerateRemoveImageTagTasks(ITagManager tagManager, IEnumerable<EntryViewModel> entries, params string[] tagNames)
        {
            List<Task> tasks = new List<Task>();

            foreach (var tagName in tagNames)
            {
                foreach (var entry in entries)
                {
                    var book = entry as BookViewModel;
                    if (book != null)
                    {
                        var pages = PageFacade.FindByBookId(book.ID);
                        for (int i = 0; i < pages.Count(); ++i)
                        {
                            var p = pages.ElementAt(i);
                            tasks.Add(new Task(() =>
                            {
                                GetPropertyIfImageIsNull(ref p);
                            }));
                            tasks.Add(new Task(() =>
                            {
                                Remove.ImageTagRemoving.Remove(tagManager, p.Image, tagName);
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
                        }));
                        tasks.Add(new Task(() =>
                        {
                            Remove.ImageTagRemoving.Remove(tagManager, page.Image, tagName);
                        }));
                        continue;
                    }

                    var image = entry as ImageViewModel;
                    if (image != null)
                    {
                        tasks.Add(new Task(() =>
                        {
                            Remove.ImageTagRemoving.Remove(tagManager, page.Image, tagName);
                        }));
                        continue;
                    }
                }

                tasks.Add(new Task(() => tagManager.SelectedEntityTags = tagManager.GetCommonTags()));
                tasks.Add(new Task(() => tagManager.ObserveSelectedEntityTags()));
                tasks.Add(new Task(() => tagManager.ObserveTagCount()));
            }

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