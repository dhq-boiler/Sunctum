

using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sunctum.Domain.Logic.Load
{
    public static class ContentsLoadTask
    {
        public static void FillContents(IBookStorage bookStorage, BookViewModel book)
        {
            var pages = PageFacade.FindByBookId(book.ID).OrderBy(p => p.PageIndex);

            bookStorage.AccessDispatcherObject(async () => book.ClearContents());

            bookStorage.AccessDispatcherObject(async () =>
            {
                foreach (var page in pages)
                {
                    book.AddPage(page);
                }
            });
        }

        public static async Task FillContentsWithImage(IBookStorage bookStorage, BookViewModel book)
        {
            var pages = PageFacade.FindByBookId(book.ID).OrderBy(p => p.PageIndex).ToList();

            bookStorage.AccessDispatcherObject(async () => book.ClearContents());

            bookStorage.AccessDispatcherObject(async () =>
            {
                foreach (var page in pages)
                {
                    await Load(page);
                    book.AddPage(page);
                }
            });
        }

        public static async Task Load(PageViewModel page)
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
        }

        private static void LoadPage(PageViewModel pageViewModel)
        {
            PageFacade.GetProperty(ref pageViewModel);
        }

        [Obsolete]
        private static async void GenerateThumbnailIf(PageViewModel page)
        {
            if (page.Image != null)
            {
                if (!page.Image.ThumbnailLoaded || !page.Image.ThumbnailGenerated)
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var tg = new Async.ThumbnailGenerating();
                        tg.Target = page.Image;
                        await (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.Enqueue(tg.GetTaskSequence());
                    });
                }
            }
        }

        internal static void SetImageToPage(PageViewModel page)
        {
            page.Image = ImageFacade.FindBy(page.ImageID);
        }
    }
}
