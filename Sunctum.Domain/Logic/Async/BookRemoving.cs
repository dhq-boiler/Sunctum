

using Homura.ORM;
using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class BookRemoving : AsyncTaskMakerBase, IBookRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ILibrary LibraryManager { get; set; }

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
                LibraryManager.RunFillContentsWithImage(book);

                book.CurrentProcessProgress.Value.TotalCount.Value = book.Contents.Count;
                book.IsDeleting = true;

                foreach (var page in book.Contents)
                {
                    sequence.Add(new Task(() => RemoveImageTagByImage(LibraryManager, page)));
                    sequence.Add(new Task(() => PageRemoving.DeleteRecordFromStorage(page)));
                    sequence.Add(new Task(() => PageRemoving.DeleteFileFromStorage(page)));
                    sequence.Add(new Task(() => ProcessedCount++));
                    sequence.Add(new Task(() => book.CurrentProcessProgress.Value.Count.Value = ProcessedCount));
                }

                sequence.Add(new Task(() => DeleteRecordFromStorage(book)));
                sequence.Add(new Task(() => LibraryManager.RemoveFromMemory(book)));
                sequence.Add(new Task(() => s_logger.Info($"Removed Book:{book}")));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookRemoving"));
        }

        private static void RemoveImageTagByImage(ILibrary libraryManager, PageViewModel page)
        {
            try
            {
                libraryManager.TagMng.RemoveByImage(page.Image);
            }
            catch (ArgumentNullException)
            {
                s_logger.Warn($"No set Image to Page. (page:{page})");
            }
        }

        private static void DeleteRecordFromStorage(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            Debug.Assert(book != null);
            BookFacade.DeleteWhereIDIs(book.ID, dataOpUnit);
        }
    }
}
