

using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Load
{
    public static class ContentsLoadTask
    {
        public static void FillContents(IBookStorage bookStorage, BookViewModel book)
        {
            var pages = PageFacade.FindByBookId(book.ID).OrderBy(p => p.PageIndex);

            bookStorage.AccessDispatcherObject(() => book.ClearContents());

            bookStorage.AccessDispatcherObject(() =>
            {
                foreach (var page in pages)
                {
                    book.AddPage(page);
                }
            });
        }

        public static void FillContentsWithImage(IBookStorage bookStorage, BookViewModel book)
        {
            var pages = PageFacade.FindByBookId(book.ID).OrderBy(p => p.PageIndex);

            bookStorage.AccessDispatcherObject(() => book.ClearContents());

            bookStorage.AccessDispatcherObject(() =>
            {
                foreach (var page in pages)
                {
                    Load(page);
                    book.AddPage(page);
                }
            });
        }

        public static void Load(PageViewModel page)
        {
            if (!page.IsLoaded)
            {
                if (page.Image == null
                    || !page.Image.ThumbnailLoaded
                    || !page.Image.ThumbnailGenerated)
                {
                    page.IsLoaded = true;
                    LoadPage(page);
                }
                else
                {
                    page.IsLoaded = true;
                }
            }

            if (Configuration.ApplicationConfiguration.ThumbnailParallelGeneration)
            {
                Task.Factory.StartNew(() => GenerateThumbnailIf(page));
            }
            else
            {
                GenerateThumbnailIf(page);
            }
        }

        private static void LoadPage(PageViewModel pageViewModel)
        {
            PageFacade.GetProperty(ref pageViewModel);
        }

        private static void GenerateThumbnailIf(PageViewModel page)
        {
            if (page.Image != null)
            {
                if (!page.Image.ThumbnailLoaded || !page.Image.ThumbnailGenerated)
                {
                    ThumbnailGenerating.GenerateThumbnail(page.Image);
                }
            }
        }

        internal static void SetImageToPage(PageViewModel page)
        {
            page.Image = ImageFacade.FindBy(page.ImageID);
        }
    }
}
