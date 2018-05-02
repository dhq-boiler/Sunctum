

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class ImageTagRemoving : AsyncTaskMakerBase, IImageTagRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<EntryViewModel> Entries { get; set; }

        [Inject]
        public ITagManager TagManager { get; set; }

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
                                Remove.ImageTagRemoving.Remove(TagManager, p.Image, tagName);
                            }));
                        }
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
                            Remove.ImageTagRemoving.Remove(TagManager, page.Image, tagName);
                        }));
                        continue;
                    }

                    var image = entry as ImageViewModel;
                    if (image != null)
                    {
                        sequence.Add(new Task(() =>
                        {
                            Remove.ImageTagRemoving.Remove(TagManager, page.Image, tagName);
                        }));
                        continue;
                    }
                }

                sequence.Add(new Task(() => TagManager.SelectedEntityTags = TagManager.GetCommonTags()));
                sequence.Add(new Task(() => TagManager.ObserveSelectedEntityTags()));
                sequence.Add(new Task(() => TagManager.ObserveTagCount()));
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