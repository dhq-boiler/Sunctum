
using Homura.ORM;
using NLog;
using Reactive.Bindings;
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

        public static void Load(BookViewModel book, DataOperationUnit dataOpUnit = null)
        {
            if (!book.IsLoaded)
            {
                if (book.FirstPage?.Image == null || book.FirstPage?.Image?.Thumbnail == null)
                {
                    LoadFirstPageAndThumbnail(book, dataOpUnit);
                    if (book.FirstPage?.Image != null && (book.FirstPage.Image.IsEncrypted || book.FirstPage.Image.ThumbnailLoaded))
                    {
                        book.IsLoaded = true;
                    }
                    var firstPageIsNotNull = book.FirstPage is not null && book.FirstPage.Image is not null;
                    if (firstPageIsNotNull && !book.FirstPage.Image.IsEncrypted && !book.FirstPage.Image.ThumbnailGenerated)
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
