

using NLog;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class PageThumbnailRemaking : AsyncTaskMakerBase, IPageThumbnailRemaking
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<PageViewModel> TargetPages { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start PageThumbnailRemaking"));
            sequence.Add(() => s_logger.Info($"      TargetBooks : {TargetPages.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var page in TargetPages)
            {
                ImageViewModel image = page.Image;
                sequence.Add(new Task(() => Delete.ThumbnailDeleting.DeleteThumbnail(image)));
                var tg = new Async.ThumbnailGenerating();
                tg.Target = image;
                sequence.AddRange(tg.GetTaskSequence().Tasks);
                sequence.Add(new Task(() => s_logger.Info($"Remade Thumbnail imageId:{image.ID}")));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish PageThumbnailRemaking"));
        }

        [Obsolete]
        public static List<Task> GenerateRemakeThumbnailTasks(IEnumerable<PageViewModel> pages)
        {
            List<Task> tasks = new List<Task>();

            foreach (var page in pages)
            {
                ImageViewModel image = page.Image;
                tasks.Add(new Task(() => Delete.ThumbnailDeleting.DeleteThumbnail(image)));
                tasks.Add(new Task(async () => await Generate.ThumbnailGenerating.GenerateThumbnail(image)));
                tasks.Add(new Task(() => s_logger.Info($"Remade Thumbnail imageId:{image.ID}")));
            }

            return tasks;
        }
    }
}
