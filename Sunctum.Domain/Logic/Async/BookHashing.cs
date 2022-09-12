using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class BookHashing : AsyncTaskMakerBase, IBookHashing
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        public UpdateRange Range { get; set; }

        public abstract class UpdateRange
        {
            public static readonly UpdateRange IsAll = new All();
            public static readonly UpdateRange IsStillNull = new StillNull();

            public abstract IEnumerable<BookViewModel> FindBook(IBookStorage bookStorage);
        }

        #region private class

        private class All : UpdateRange
        {
            public override IEnumerable<BookViewModel> FindBook(IBookStorage bookStorage)
            {
                return bookStorage.BookSource;
            }
        }

        private class StillNull : UpdateRange
        {
            public override IEnumerable<BookViewModel> FindBook(IBookStorage bookStorage)
            {
                return bookStorage.BookSource.Where(b => b.FingerPrint == null).Reverse();
            }
        }

        #endregion //private class

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookHashing"));
            sequence.Add(() => s_logger.Info($"      UpdateRange : {Range.GetType().Name}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(new Task(() => s_logger.Info($"Begin to hash book.")));

            var books = Range.FindBook(LibraryManager.Value);

            sequence.Add(new Task(() => s_logger.Info($"Found : {books.Count()}")));

            foreach (var book in books)
            {
                sequence.AddRange(GenerateTasksToCalcBook(LibraryManager.Value, book));
            }

            sequence.Add(new Task(() => s_logger.Info($"Finished to hash book.")));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookHashing"));
        }

        private IEnumerable<Task> GenerateTasksToCalcBook(ILibrary libVM, BookViewModel book)
        {
            List<Task> ret = new List<Task>();
            ret.Add(new Task(() => ExtractFingerPrint(book)));
            ret.Add(new Task(() => UpdateBook(book)));

            return ret;
        }

        private void ExtractFingerPrint(BookViewModel book)
        {
            ContentsLoadTask.FillContentsWithImage(LibraryManager.Value, book);
            book.FingerPrint = Hash.Generate(book);
        }

        private void GetImage(PageViewModel page)
        {
            PageFacade.GetProperty(ref page);
        }


        private void UpdateBook(BookViewModel book)
        {
            BookFacade.Update(book);
        }
    }
}
