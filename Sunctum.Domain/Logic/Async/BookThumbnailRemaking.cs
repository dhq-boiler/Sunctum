﻿

using NLog;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class BookThumbnailRemaking : AsyncTaskMakerBase, IBookThumbnailRemaking
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<BookViewModel> TargetBooks { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookThumbnailRemaking"));
            sequence.Add(() => s_logger.Info($"      TargetBooks : {TargetBooks.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var book in TargetBooks)
            {
                if (book.FirstPage == null) continue;

                ImageViewModel image = book.FirstPage.Image;
                sequence.Add(new Task(() => Delete.ThumbnailDeleting.DeleteThumbnail(image)));
                sequence.Add(new Task(() => ThumbnailGenerating.GenerateThumbnail(image)));
                sequence.Add(new Task(() => s_logger.Info($"Remade Thumbnail imageId:{image.ID}")));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookThumbnailRemaking"));
        }

        [Obsolete]
        public static List<Task> GenerateRemakeThumbnailTasks(IEnumerable<BookViewModel> books)
        {
            List<Task> tasks = new List<Task>();

            foreach (var book in books)
            {
                if (book.FirstPage == null) continue;

                ImageViewModel image = book.FirstPage.Image;
                tasks.Add(new Task(() => Delete.ThumbnailDeleting.DeleteThumbnail(image)));
                tasks.Add(new Task(() => ThumbnailGenerating.GenerateThumbnail(image)));
                tasks.Add(new Task(() => s_logger.Info($"Remade Thumbnail imageId:{image.ID}")));
            }

            return tasks;
        }
    }
}
