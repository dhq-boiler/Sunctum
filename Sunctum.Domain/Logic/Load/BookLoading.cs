
using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Load
{
    public static class BookLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Obsolete]
        public static async Task LoadBookListAsync(ILibrary libVM)
        {
            await Task.Run(() => LoadBookList(libVM));
        }

        [Obsolete]
        public static void LoadBookList(ILibrary libVM)
        {
            Stopwatch sw = new Stopwatch();
            s_logger.Info("Loading Book list...");
            sw.Start();
            using (var dataOpUnit = new DataOperationUnit())
            {
                dataOpUnit.Open(ConnectionManager.DefaultConnection);

                libVM.BookSource.CollectionChanged -= libVM.AuthorManager.LoadedBooks_CollectionChanged;

                libVM.BookSource = new ObservableCollection<BookViewModel>(BookFacade.FindAllWithAuthor(dataOpUnit));
                libVM.AuthorManager.LoadAuthorCount();
                libVM.BookSource.CollectionChanged += libVM.AuthorManager.LoadedBooks_CollectionChanged;
            }
            sw.Stop();
            s_logger.Info($"Completed to load Book list. {sw.ElapsedMilliseconds}ms");
        }

        public static void Load(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            if (!book.IsLoaded)
            {
                if (book.FirstPage?.Image == null || book.FirstPage?.Image?.Thumbnail == null)
                {
                    LoadFirstPageAndThumbnail(book, dataOpUnit);
                    if (book.FirstPage?.Image != null && (Configuration.ApplicationConfiguration.LibraryIsEncrypted || book.FirstPage.Image.ThumbnailLoaded))
                    {
                        book.IsLoaded = true;
                    }
                    var firstPageIsNull = book.FirstPage.Image != null;
                    if (!Configuration.ApplicationConfiguration.LibraryIsEncrypted && firstPageIsNull && !book.FirstPage.Image.ThumbnailGenerated)
                    {
                        GenerateThumbnailCondition(book, dataOpUnit);
                    }
                }
                else
                {
                    book.IsLoaded = true;
                }
            }
        }

        public static void GenerateThumbnailCondition(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            if (Configuration.ApplicationConfiguration.ThumbnailParallelGeneration)
            {
                Task.Factory.StartNew(() => GenerateThumbnailIf(book));
            }
            else
            {
                GenerateThumbnailIf(book, dataOpUnit);
            }
        }

        internal static void LoadAuthor(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            if (book.AuthorID != Guid.Empty)
            {
                book.Author = AuthorFacade.FindBy(book.AuthorID, dataOpUnit);
            }
        }

        public static void GenerateThumbnailIf(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            var firstPage = book.FirstPage;
            if (firstPage != null)
            {
                var firstPageImage = firstPage.Image;

                if (!firstPageImage.ThumbnailLoaded)
                {
                    firstPageImage.Thumbnail = ThumbnailFacade.FindByImageID(firstPageImage.ID, dataOpUnit);
                }

                if (!firstPageImage.ThumbnailLoaded
                    || firstPageImage.Thumbnail?.RelativeMasterPath == null
                    || !firstPageImage.ThumbnailGenerated)
                {
                    ThumbnailGenerating.GenerateThumbnail(firstPageImage, dataOpUnit);
                }
            }
        }

        private static void LoadFirstPageAndThumbnail(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            BookFacade.GetProeprty(ref book, dataOpUnit);
        }
    }
}
