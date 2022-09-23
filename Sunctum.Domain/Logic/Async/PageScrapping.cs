using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class PageScrapping : AsyncTaskMakerBase, IPageScrapping
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private int _count;
        private ImageViewModel _newimg;
        private BookViewModel _newBook;

        internal BookViewModel NewBook { get { return _newBook; } set { _newBook = value; } }

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        public string Title { get; set; }

        public IEnumerable<PageViewModel> TargetPages { get; set; }

        public string MasterDirectory { get; set; }

        public PageScrapping()
        {
            _count = 0;
        }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start PageScrapping"));
            sequence.Add(() => s_logger.Info($"      Title : {Title}"));
            sequence.Add(() => s_logger.Info($"      MasterDirectory : {MasterDirectory}"));
            sequence.Add(() => s_logger.Info($"      TargetPages : {TargetPages.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            TargetPages = TargetPages.OrderBy(a => a.Title, new NaturalStringComparer()).ToArray();

            var workingDirectory = Configuration.ApplicationConfiguration.WorkingDirectory;
            var now = DateTime.Now;

            Guid entryNameSeedGuid = Guid.NewGuid();
            var bookDir = entryNameSeedGuid.ToString("N");

            var copyTo = workingDirectory + "\\"
                + MasterDirectory + "\\"
                + now.Year + "\\"
                + now.Month.ToString("00") + "\\"
                + now.Day.ToString("00") + "\\"
                + bookDir;

            //保存先ディレクトリ準備
            if (!Directory.Exists(copyTo))
            {
                Directory.CreateDirectory(copyTo);
                s_logger.Debug($"Create directory:{copyTo}");
            }

            sequence.Add(new System.Threading.Tasks.Task(() => CreateBook(bookDir, Title)));

            var page = TargetPages.ElementAt(0);
            var img = page.Image;
            var destination = copyTo + "\\" + Path.GetFileName(img.AbsoluteMasterPath);
            AddTaskToProcessScrapPage(sequence, page, img, destination);

            sequence.Add(new System.Threading.Tasks.Task(() => SetFirstPage()));

            sequence.Add(new System.Threading.Tasks.Task(() => LibraryManager.Value.AddToMemory(NewBook)));

            //ファイルコピー
            for (int i = 1; i < TargetPages.Count(); ++i)
            {
                page = TargetPages.ElementAt(i);
                img = page.Image;
                destination = copyTo + "\\" + Path.GetFileName(img.AbsoluteMasterPath);
                AddTaskToProcessScrapPage(sequence, page, img, destination);
            }

            sequence.Add(new System.Threading.Tasks.Task(() => LibraryManager.Value.FireFillContents(NewBook)));
            sequence.Add(new System.Threading.Tasks.Task(() => s_logger.Info($"Pages Scrapped as:{NewBook}")));
            sequence.Add(new System.Threading.Tasks.Task(() => SetContentsRegistered()));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish PageScrapping"));
        }

        [Obsolete]
        public static List<System.Threading.Tasks.Task> GenerateScrapPagesTasks(ILibrary libVM, string title, ref PageViewModel[] pages, string masterDirectory)
        {
            pages = pages.OrderBy(a => a.Title, new NaturalStringComparer()).ToArray();

            var workingDirectory = Configuration.ApplicationConfiguration.WorkingDirectory;
            var now = DateTime.Now;

            Guid entryNameSeedGuid = Guid.NewGuid();
            var bookDir = entryNameSeedGuid.ToString("N");

            var copyTo = workingDirectory + "\\"
                + masterDirectory + "\\"
                + now.Year + "\\"
                + now.Month.ToString("00") + "\\"
                + now.Day.ToString("00") + "\\"
                + bookDir;

            //保存先ディレクトリ準備
            if (!Directory.Exists(copyTo))
            {
                Directory.CreateDirectory(copyTo);
                s_logger.Debug($"Create directory:{copyTo}");
            }

            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();

            PageScrapping sptask = new PageScrapping();
            tasks.Add(new System.Threading.Tasks.Task(() => sptask.CreateBook(bookDir, title)));

            var page = pages.ElementAt(0);
            var img = page.Image;
            var destination = copyTo + "\\" + Path.GetFileName(img.AbsoluteMasterPath);
            AddTaskToProcessScrapPage(tasks, sptask, page, img, destination);

            tasks.Add(new System.Threading.Tasks.Task(() => sptask.SetFirstPage()));

            tasks.Add(new System.Threading.Tasks.Task(() => libVM.AddToMemory(sptask.NewBook)));

            //ファイルコピー
            for (int i = 1; i < pages.Count(); ++i)
            {
                page = pages.ElementAt(i);
                img = page.Image;
                destination = copyTo + "\\" + Path.GetFileName(img.AbsoluteMasterPath);
                AddTaskToProcessScrapPage(tasks, sptask, page, img, destination);
            }

            tasks.Add(new System.Threading.Tasks.Task(() => libVM.FireFillContents(sptask.NewBook)));
            tasks.Add(new System.Threading.Tasks.Task(() => s_logger.Info($"Pages Scrapped as:{sptask.NewBook}")));
            tasks.Add(new System.Threading.Tasks.Task(() => sptask.SetContentsRegistered()));
            return tasks;
        }

        private void AddTaskToProcessScrapPage(AsyncTaskSequence sequence, PageViewModel page, ImageViewModel img, string destination)
        {
            page.IsScrapped = true;

            sequence.Add(() => CopyFile(img.AbsoluteMasterPath, destination));

            //Image Create
            sequence.Add(() => CreateImage(img, destination));

            //Page Create
            sequence.Add(() => CreatePage(img));

            sequence.Add(() => CountUp());
        }

        [Obsolete]
        private static void AddTaskToProcessScrapPage(List<System.Threading.Tasks.Task> tasks, PageScrapping sptask, PageViewModel page, ImageViewModel img, string destination)
        {
            page.IsScrapped = true;

            tasks.Add(new System.Threading.Tasks.Task(() => sptask.CopyFile(img.AbsoluteMasterPath, destination)));

            //Image Create
            tasks.Add(new System.Threading.Tasks.Task(() => sptask.CreateImage(img, destination)));

            //Page Create
            tasks.Add(new System.Threading.Tasks.Task(() => sptask.CreatePage(img)));

            tasks.Add(new System.Threading.Tasks.Task(() => sptask.CountUp()));
        }

        private void CreateBook(string bookDir, string title)
        {
            NewBook = new BookViewModel(Guid.NewGuid(), title);
            NewBook.Configuration = Configuration.ApplicationConfiguration;
            BookFacade.Insert(NewBook);
        }

        private void CopyFile(string source, string destination)
        {
            File.Copy(source, destination);
            s_logger.Debug($"Copy:{source}");
        }

        private void CreateImage(ImageViewModel img, string destination)
        {
            _newimg = new ImageViewModel(Guid.NewGuid(), img.Title, destination, img.IsEncrypted, Configuration.ApplicationConfiguration);
            ImageFacade.Insert(_newimg);
        }

        private void CreatePage(ImageViewModel img)
        {
            PageViewModel _newpage = new PageViewModel(Guid.NewGuid(), img.Title, _count + 1);
            _newpage.Configuration = Configuration.ApplicationConfiguration;
            _newpage.ImageID = _newimg.ID;
            _newpage.Image = _newimg;
            _newpage.BookID = NewBook.ID;
            PageFacade.Insert(_newpage);
        }

        private void CountUp()
        {
            ++_count;
        }

        private void SetFirstPage()
        {
            var page = PageFacade.FindByBookIdTop1(NewBook.ID);
            page.Image = ImageFacade.FindBy(page.ImageID);

            NewBook.FirstPage.Value = page;
            s_logger.Debug($"Set FirstPage : {page.Image}");

            Load.BookLoading.GenerateThumbnailIf(NewBook);
        }

        private void SetContentsRegistered()
        {
            NewBook.ContentsRegistered = true;
        }
    }
}
