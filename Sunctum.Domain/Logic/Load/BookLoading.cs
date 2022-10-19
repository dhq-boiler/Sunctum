
using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Sunctum.Domain.Logic.Load
{
    public static class BookLoading
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Load(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            if (!book.IsLoaded)
            {
                if (book.FirstPage.Value?.Image == null || book.FirstPage.Value?.Image?.Thumbnail == null)
                {
                    LoadFirstPageAndThumbnail(book, dataOpUnit);
                    if (book.FirstPage.Value?.Image != null && (book.FirstPage.Value.Image.IsEncrypted || book.FirstPage.Value.Image.ThumbnailLoaded))
                    {
                        book.IsLoaded = true;
                    }
                    var firstPageIsNotNull = book.FirstPage is not null && book.FirstPage.Value is not null && book.FirstPage.Value.Image is not null;
                    if (firstPageIsNotNull && book.FirstPage.Value is not null && !book.FirstPage.Value.Image.IsEncrypted && !book.FirstPage.Value.Image.ThumbnailGenerated)
                    {
                        GenerateThumbnailCondition(book, dataOpUnit);
                    }
                }
                else
                {
                    book.IsLoaded = true;
                }
            }

            if (book.FirstPage.Value is null)
            {
                s_logger.Error("book.FirstPage.Value is null");
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

        public static async void GenerateThumbnailIf(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            var firstPage = book.FirstPage;
            if (firstPage != null)
            {
                var firstPageImage = firstPage.Value.Image;

                if (!firstPageImage.ThumbnailLoaded)
                {
                    firstPageImage.Thumbnail = ThumbnailFacade.FindByImageID(firstPageImage.ID, dataOpUnit);
                }

                if (!firstPageImage.ThumbnailLoaded
                    || firstPageImage.Thumbnail?.RelativeMasterPath == null
                    || !firstPageImage.ThumbnailGenerated)
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var tg = new Async.ThumbnailGenerating();
                        tg.Target = firstPageImage;
                        await (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.Enqueue(tg.GetTaskSequence());
                    });
                }
            }
        }

        private static void LoadFirstPageAndThumbnail(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            BookFacade.GetProeprty(ref book, dataOpUnit);
        }
    }
}
