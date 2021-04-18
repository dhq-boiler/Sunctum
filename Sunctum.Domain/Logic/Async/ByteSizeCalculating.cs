

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class ByteSizeCalculating : AsyncTaskMakerBase, IByteSizeCalculating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private long _byteSize;

        [Inject]
        public ILibrary LibraryManager { get; set; }

        public UpdateRange Range { get; set; }

        public abstract class UpdateRange
        {
            public static readonly UpdateRange IsAll = new All();
            public static readonly UpdateRange IsStillNull = new StillNull();

            public abstract IEnumerable<BookViewModel> FindBook(ILibrary library);
        }

        #region private class

        private class All : UpdateRange
        {
            public override IEnumerable<BookViewModel> FindBook(ILibrary library)
            {
                return library.BookSource;
            }
        }

        private class StillNull : UpdateRange
        {
            public override IEnumerable<BookViewModel> FindBook(ILibrary library)
            {
                return library.BookSource.Where(b => b.ByteSize == null);
            }
        }

        #endregion //private class

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start ByteSizeCalculating"));
            sequence.Add(() => s_logger.Info($"      UpdateRange : {Range.GetType().Name}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(new Task(() => s_logger.Info($"Begin to Calculate Book ByteSize.")));

            var books = Range.FindBook(LibraryManager);

            sequence.Add(new Task(() => s_logger.Info($"Found : {books.Count()}")));

            foreach (var book in books)
            {
                sequence.AddRange(GenerateTasksToCalcBook(LibraryManager, book));
            }

            sequence.Add(new Task(() => s_logger.Info($"Finished to Calculate Book ByteSize.")));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ByteSizeCalculating"));
        }

        private IEnumerable<Task> GenerateTasksToCalcBook(ILibrary libVM, BookViewModel book)
        {
            List<Task> ret = new List<Task>();

            var pages = PageFacade.FindByBookId(book.ID).OrderBy(p => p.PageIndex);

            foreach (var page in pages)
            {
                ret.Add(new Task(() => GetImage(page)));
                ret.Add(new Task(() => UpdateImage(page.Image)));
                ret.Add(new Task(() => AddByteSize(page.Image)));
            }

            ret.Add(new Task(() => UpdateBook(book)));
            ret.Add(new Task(() => ResetByteSize()));

            return ret;
        }

        private void GetImage(PageViewModel page)
        {
            PageFacade.GetProperty(ref page);
        }

        private void UpdateImage(ImageViewModel image)
        {
            image.ByteSize = image.MasterFileSize;
            ImageFacade.Update(image);
        }

        private void AddByteSize(ImageViewModel image)
        {
            _byteSize += image.ByteSize.Value;
        }

        private void UpdateBook(BookViewModel book)
        {
            book.ByteSize = _byteSize;
            BookFacade.Update(book);
        }

        private void ResetByteSize()
        {
            _byteSize = 0;
        }
    }
}
