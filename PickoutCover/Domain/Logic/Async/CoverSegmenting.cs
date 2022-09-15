

using ChinhDo.Transactions;
using Homura.ORM;
using NLog;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using PickoutCover.Domain.Logic.CoverSegment;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.Logic.Query;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Imaging;

namespace PickoutCover.Domain.Logic.Async
{
    public class CoverSegmenting : AsyncTaskMakerBase, IAsyncTaskMaker
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private string _masterPath;
        private BookViewModel _book;
        private ILibrary libraryVM;
        private PageViewModel page;
        private CoverSideCandidate coverLeftSide;
        private CoverSideCandidate coverRightSide;
        private DataOperationUnit trans;

        public CoverSegmenting(ILibrary libraryVM, PageViewModel page, CoverSideCandidate coverLeftSide, CoverSideCandidate coverRightSide, DataOperationUnit trans)
        {
            this.libraryVM = libraryVM;
            this.page = page;
            this.coverLeftSide = coverLeftSide;
            this.coverRightSide = coverRightSide;
            this.trans = trans;
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(new Task(() => IncrementPageIndexExistingPagesPage(page.BookID, trans)));
            sequence.Add(new Task(() => GenerateNewCoverBitmap(page, coverLeftSide, coverRightSide, this, trans)));
            sequence.Add(new Task(() => CreateEntities(libraryVM, page, this, trans)));
            sequence.Add(new Task(() => libraryVM.FireFillContents(this._book)));
            sequence.Add(new Task(() => s_logger.Info($"Generated Cover Page:{page}")));
        }

        //public static List<Task> GenerateCoverSegmentTasks(ILibrary _libraryVM, PageViewModel _page, CoverSideCandidate CoverLeftSide, CoverSideCandidate CoverRightSide, DataOperationUnit dataOpUnit)
        //{
        //    List<Task> tasks = new List<Task>();

        //    CoverSegmenting cs = new CoverSegmenting();
        //    tasks.Add(new Task(() => IncrementPageIndexExistingPagesPage(_page.BookID, dataOpUnit)));
        //    tasks.Add(new Task(() => GenerateNewCoverBitmap(_page, CoverLeftSide, CoverRightSide, cs, dataOpUnit)));
        //    tasks.Add(new Task(() => CreateEntities(_libraryVM, _page, cs, dataOpUnit)));
        //    tasks.Add(new Task(() => _libraryVM.FireFillContents(cs._book)));
        //    tasks.Add(new Task(() => s_logger.Info($"Generated Cover Page:{_page}")));

        //    return tasks;
        //}

        private static void IncrementPageIndexExistingPagesPage(Guid bookID, DataOperationUnit dataOpUnit)
        {
            PageFacade.IncrementPageIndex(bookID, dataOpUnit);
        }

        private static void CreateEntities(ILibrary _libraryVM, PageViewModel _page, CoverSegmenting cs, DataOperationUnit dataOpUnit)
        {
            Guid imageID = Guid.NewGuid();
            var image = new ImageViewModel(imageID, "cover", cs._masterPath, Configuration.ApplicationConfiguration.LibraryIsEncrypted, Configuration.ApplicationConfiguration);
            image.Configuration = Configuration.ApplicationConfiguration;
            image.ID = imageID;
            image.UnescapedTitle = "cover";
            if (System.IO.Path.IsPathRooted(cs._masterPath))
            {
                image.RelativeMasterPath = ImageViewModel.MakeRelativeMasterPath(Configuration.ApplicationConfiguration.WorkingDirectory, cs._masterPath);
            }
            else
            {
                image.RelativeMasterPath = cs._masterPath;
            }
            ImageFacade.Insert(image, dataOpUnit);

            var pageID = Guid.NewGuid();
            var page = new PageViewModel();
            page.Configuration = Configuration.ApplicationConfiguration;
            page.ID = pageID;
            page.UnescapedTitle = "cover";
            page.ImageID = imageID;
            page.BookID = _page.BookID;
            page.PageIndex = 1;
            page.Image = image;
            PageFacade.Insert(page, dataOpUnit);

            var encryptImage = EncryptImageFacade.FindBy(image.ID);
            if (Configuration.ApplicationConfiguration.LibraryIsEncrypted && encryptImage is not null)
            {
                using (var scope = new TransactionScope())
                {
                    var fileManager = new TxFileManager();
                    Encryptor.Encrypt(page.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{page.Image.ID.ToString().Substring(0, 2)}\\{page.Image.ID}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", Configuration.ApplicationConfiguration.Password, fileManager);
                    scope.Complete();
                }
            }

            var book = _libraryVM.OnStage.Where(b => b.ID.Equals(_page.BookID)).Single();
            book.FirstPage = page;
            if (book.FirstPage.Image.ThumbnailRecorded)
            {
                book.FirstPage.Image.Thumbnail = ThumbnailFacade.FindByImageID(book.FirstPage.ImageID, dataOpUnit);
            }

            if (!book.FirstPage.Image.ThumbnailLoaded || !book.FirstPage.Image.ThumbnailGenerated)
            {
                ThumbnailGenerating.GenerateThumbnail(book.FirstPage.Image, dataOpUnit);
            }

            cs._book = book;
        }

        private static void GenerateNewCoverBitmap(PageViewModel _page, CoverSideCandidate CoverLeftSide, CoverSideCandidate CoverRightSide, CoverSegmenting cs, DataOperationUnit dataOpUnit)
        {
            int count = Querying.BookContentsCount(_page.BookID, dataOpUnit);
            var dir = Path.GetDirectoryName(_page.Image.AbsoluteMasterPath);
            cs._masterPath = $"{dir}\\XC{(count + 1)}.jpg";
            var newBitmap = CopyBitmap(_page, CoverLeftSide, CoverRightSide);
            SaveBitmap(newBitmap, cs._masterPath);
        }

        private static void SaveBitmap(WriteableBitmap target, string filename)
        {
            using (FileStream stream = new FileStream(filename,
                                        FileMode.Create, FileAccess.Write))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(target));
                encoder.QualityLevel = 100;
                encoder.Save(stream);
            }
        }

        private static unsafe WriteableBitmap CopyBitmap(PageViewModel _page, CoverSideCandidate CoverLeftSide, CoverSideCandidate CoverRightSide)
        {
            using (Mat mat = new Mat(_page.Image.AbsoluteMasterPath, ImreadModes.Unchanged))
            using (Mat ext = ExtractRect(mat, new OpenCvSharp.Rect(CoverLeftSide.Offset, 0, CoverRightSide.Offset - CoverLeftSide.Offset + 1, mat.Height)))
            {
                return WriteableBitmapConverter.ToWriteableBitmap(ext);
            }
        }

        public static unsafe Mat ExtractRect(Mat src, OpenCvSharp.Rect position)
        {
            Mat ret = new Mat(position.Height, position.Width, MatType.CV_8UC3);
            int top = position.Top;
            int bottom = position.Bottom;
            int left = position.Left;
            int right = position.Right;
            int src_channels = src.Channels();
            long ret_step = ret.Step();
            long src_step = src.Step();
            int src_height = src.Height;
            int src_width = src.Width;
            byte* ps = (byte*)src.Data.ToPointer();
            byte* pr = (byte*)ret.Data.ToPointer();

#if RELEASE_PARALLEL
            Parallel.For(0, src_height, y =>
            {
                if (top <= y && y <= bottom)
                {
                    for (int x = 0; x < src_width; ++x)
                    {
                        if (left <= x && x <= right)
                        {
                            for (int c = 0; c < src_channels; ++c)
                            {
                                *(pr + (y - top) * ret_step + (x - left) * src_channels + c) = *(ps + y * src_step + x * src_channels + c);
                            }
                        }
                    }
                }
            });
#else
            for (int y = 0; y < src_height; ++y)
            {
                if (top <= y && y <= bottom)
                {
                    for (int x = 0; x < src_width; ++x)
                    {
                        if (left <= x && x <= right)
                        {
                            for (int c = 0; c < src_channels; ++c)
                            {
                                *(pr + (y - top) * ret_step + (x - left) * src_channels + c) = *(ps + y * src_step + x * src_channels + c);
                            }
                        }
                    }
                }
            }
#endif

            return ret;
        }
    }
}
