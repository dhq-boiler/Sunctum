﻿

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class BookRemoving : AsyncTaskMakerBase, IBookRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        public IEnumerable<BookViewModel> TargetBooks { get; set; }

        public int ProcessedCount { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookRemoving"));
            sequence.Add(() => s_logger.Info($"      TargetBooks : {TargetBooks.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var book in TargetBooks)
            {
                LibraryManager.Value.RunFillContentsWithImage(book);

                book.IsDeleting = true;
                sequence.Add(() => book.CurrentProcessProgress.Value.TotalCount.Value = book.Contents.Count);
                sequence.Add(() => book.CurrentProcessProgress.Value.Count.Value = 0);
                sequence.Add(() => ProcessedCount = 0);

                foreach (var page in book.Contents)
                {
                    sequence.Add(new Task(async () => await RemoveImageTagByImage(LibraryManager.Value, page).ConfigureAwait(false)));
                    sequence.Add(new Task(async () => await PageRemoving.DeleteFileFromStorage(page).ConfigureAwait(false)));
                    sequence.Add(new Task(async () => await PageRemoving.DeleteRecordFromStorage(page).ConfigureAwait(false)));
                    sequence.Add(new Task(() => ProcessedCount++));
                    sequence.Add(new Task(() => book.CurrentProcessProgress.Value.Count.Value = ProcessedCount));
                }

                sequence.Add(new Task(async () => await DeleteRecordFromStorage(book).ConfigureAwait(false)));
                sequence.Add(new Task(() => LibraryManager.Value.RemoveFromMemory(book)));
                sequence.Add(new Task(() => s_logger.Info($"Removed Book:{book}")));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookRemoving"));
        }

        private static async Task RemoveImageTagByImage(ILibrary libraryManager, PageViewModel page)
        {
            try
            {
                await libraryManager.TagManager.RemoveByImage(page.Image).ConfigureAwait(false);
            }
            catch (ArgumentNullException)
            {
                s_logger.Warn($"No set Image to Page. (page:{page})");
            }
        }

        private static async Task DeleteRecordFromStorage(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            Debug.Assert(book != null);
            await BookFacade.DeleteWhereIDIs(book.ID, dataOpUnit).ConfigureAwait(false);
        }
    }
}
