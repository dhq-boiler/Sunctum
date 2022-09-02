

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class PageRemoving : AsyncTaskMakerBase, IPageRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        public IEnumerable<PageViewModel> TargetPages { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start PageRemoving"));
            sequence.Add(() => s_logger.Info($"      TargetPages : {TargetPages.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var page in TargetPages)
            {
                sequence.Add(new Task(() => ContentsLoadTask.SetImageToPage(page)));
                sequence.Add(new Task(() => DeleteFileFromStorage(page)));
                sequence.Add(new Task(() => DeleteRecordFromStorage(page)));
                sequence.Add(new Task(() => RemovePageFromBook(LibraryManager.Value, page)));
                sequence.Add(new Task(() => s_logger.Info($"Removed Page:{page}")));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish PageRemoving"));
        }

        [Obsolete]
        public static List<Task> GenerateRemoveTasks(ILibrary libVM, PageViewModel[] pages, DataOperationUnit dataOpUnit)
        {
            List<Task> tasks = new List<Task>();

            foreach (var page in pages)
            {
                tasks.Add(new Task(() => ContentsLoadTask.SetImageToPage(page)));
                tasks.Add(new Task(() => DeleteFileFromStorage(page)));
                tasks.Add(new Task(() => DeleteRecordFromStorage(page, dataOpUnit)));
                tasks.Add(new Task(() => RemovePageFromBook(libVM, page)));
                tasks.Add(new Task(() => s_logger.Info($"Removed Page:{page}")));
            }

            return tasks;
        }

        private static void RemovePageFromBook(ILibrary libVM, PageViewModel page)
        {
            var bookInLib = libVM.BookSource.Where(b => b.ID == page.BookID).Single();
            libVM.AccessDispatcherObject(() => bookInLib.RemovePage(page));
        }

        internal static void DeleteFileFromStorage(PageViewModel page)
        {
            if (page == null)
            {
                s_logger.Error($"page is null");
                return;
            }

            if (page.Image == null)
            {
                s_logger.Warn($"No set Image to Page. (page:{page})");
                return;
            }

            var filename = page.Image.AbsoluteMasterPath;

            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    s_logger.Debug($"Delete image file: {filename}");
                }
                catch (IOException e)
                {
                    s_logger.Error($"Failed to delete image file. (filename:{filename}, page:{page})");
                    s_logger.Debug(e);
                    throw;
                }
            }

            if (Configuration.ApplicationConfiguration.LibraryIsEncrypted)
            {
                var encryptImage = EncryptImageFacade.FindBy(page.ImageID);
                if (encryptImage is not null && File.Exists(encryptImage.EncryptFilePath))
                {
                    try
                    {
                        File.Delete(encryptImage.EncryptFilePath);
                        s_logger.Debug($"Delete image file: {encryptImage.EncryptFilePath}");
                    }
                    catch (IOException e)
                    {
                        s_logger.Error($"Failed to delete image file. (filename:{encryptImage.EncryptFilePath}, page:{page})");
                        s_logger.Debug(e);
                        throw;
                    }
                }
            }

            if (page.Image.ThumbnailGenerated)
            {
                filename = page.Image.Thumbnail.AbsoluteMasterPath;

                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                        s_logger.Debug($"Delete thumbnail file: {filename}");
                    }
                    catch (IOException e)
                    {
                        s_logger.Error($"Failed to delete thumbnail file. (filename:{filename}, page:{page})");
                        s_logger.Debug(e);
                        throw;
                    }
                }
            }
        }

        internal static void DeleteRecordFromStorage(PageViewModel page, DataOperationUnit dataOpUnit = null)
        {
            Debug.Assert(page != null);

            if (Configuration.ApplicationConfiguration.LibraryIsEncrypted)
            {
                EncryptImageFacade.DeleteBy(page.ImageID);
            }

            PageFacade.DeleteWhereIDIs(page.ID, dataOpUnit);
            ImageFacade.DeleteWhereIDIs(page.ImageID, dataOpUnit);
            if (page.Image != null)
            {
                var image = page.Image;
                if (image.ThumbnailRecorded)
                {
                    if (!image.ThumbnailLoaded)
                    {
                        image.Thumbnail = ThumbnailFacade.FindByImageID(image.ID, dataOpUnit);
                    }
                    ThumbnailFacade.DeleteWhereIDIs(image.Thumbnail.ID, dataOpUnit);
                }
            }
        }
    }
}
