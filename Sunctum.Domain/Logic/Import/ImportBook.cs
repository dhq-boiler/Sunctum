

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Import
{
    internal class ImportBook : Importer
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        protected List<Importer> _children;
        protected BookViewModel _book;
        private Guid _AuthorID;
        private Guid _BookID;
        private List<ImageViewModel> _images;

        public List<string> TagNames { get; set; } = new List<string>();

        public string AuthorName { get; set; }

        public string Title { get; set; }

        public ILibrary LibraryManager { get; set; }

        internal ImportBook()
        { }

        internal ImportBook(string path, ILibrary library)
            : base(path)
        {
            LibraryManager = library;
        }

        public override string ToString()
        {
            return "Import Directory from " + Path;
        }

        public override void Estimate()
        {
            string[] files = Directory.GetFiles(Path).OrderBy(a => a, new NaturalStringComparer()).ToArray();
            var targets = files.Where(a => !File.GetAttributes(a).HasFlag(FileAttributes.Hidden) && Specifications.SupportedImageType.Any(b => a.ToLower().EndsWith(b.ToLower())));

            _children = new List<Importer>();
            foreach (var target in targets)
            {
                _children.Add(new ImportPage(target, true));
            }
            Count = _children.Count();
        }

        public override IEnumerable<Task> GenerateTasks(ILibrary library, string copyTo, string entryName, DataOperationUnit dataOpUnit, Action<Importer, BookViewModel> progressUpdatingAction)
        {
            List<Task> ret = new List<Task>();

            var directoryPath = copyTo + "\\" + entryName;

            ret.Add(new Task(() => CreateTaskToCreateDirectoryIfDoesntExist(directoryPath)));

            if (!string.IsNullOrWhiteSpace(AuthorName))
            {
                ret.Add(new Task(() => CreateTaskToInsertAuthor(AuthorName, dataOpUnit)));
            }

            ret.Add(new Task(() => CreateTaskToInsertBook(entryName, Title, dataOpUnit)));

            Processed = 0;

            var firstChild = _children.First();
            ProcessChildren(library, ret, directoryPath, firstChild, dataOpUnit, progressUpdatingAction);
            ret.Add(new Task(() => GenerateDeliverables(dataOpUnit)));
            ret.Add(new Task(() => SetDeliverables(library)));

            for (int i = 1; i < _children.Count(); ++i)
            {
                var child = _children[i];
                ProcessChildren(library, ret, directoryPath, child, dataOpUnit, progressUpdatingAction);
            }

            ret.Add(new Task(() => WriteMetadata()));

            ret.Add(new Task(() => TagImage(library.TagManager)));

            ret.Add(new Task(() => TagBook(library.TagManager)));

            ret.Add(new Task(() => SwitchContentsRegisteredToTrue()));

            ret.Add(new Task(() => Log()));

            return ret;
        }

        protected void WriteMetadata()
        {
            _book.ByteSize = 0;
            _children.ForEach(c => _book.ByteSize += ((ImportPage)c).Size);
            _book.FingerPrint = Hash.Generate(_children);
            BookFacade.Update(_book);
        }

        private void TagBook(ITagManager tagMng)
        {
            foreach (var tagName in TagNames)
            {
                var tag = TagFacade.FindByTagName(tagName);
                Debug.Assert(tag != null);
                var newBookTag = new BookTagViewModel(_book, tag);

                BookTagFacade.Insert(newBookTag);
                tagMng.BookTagChains.Add(newBookTag);
            }
        }

        private void TagImage(ITagManager tagManager)
        {
            foreach (var tagName in TagNames)
            {
                var tag = TagFacade.FindByTagName(tagName);

                if (tag == null)
                {
                    tag = new TagViewModel(Guid.NewGuid(), tagName);
                    TagFacade.Insert(tag);
                    tagManager.Tags.Add(tag);
                }

                _images = new List<ImageViewModel>();

                var pages = PageFacade.FindByBookId(_BookID);
                for (int i = 0; i < pages.Count(); ++i)
                {
                    var page = pages.ElementAt(i);
                    PageFacade.GetProperty(ref page);
                    _images.Add(page.Image);
                }

                ImageTagFacade.BatchInsert(tag, _images);

                foreach (var imageTag in _images.Select(i => new ImageTagViewModel(i.ID, tag)))
                {
                    tagManager.Chains.Add(imageTag);
                }
            }
        }

        protected void SwitchContentsRegisteredToTrue()
        {
            _book.ContentsRegistered = true;
        }

        private void ProcessChildren(ILibrary library, List<Task> ret, string directoryPath, Importer child, DataOperationUnit dataOpUnit, Action<Importer, BookViewModel> progressUpdatingAction)
        {
            if (child is ImportPage)
            {
                var ip = child as ImportPage;
                ip.PageIndex = Processed + Specifications.PAGEINDEX_FIRSTPAGE;
                ip.TotalPageCount = _children.Count();
                ip.PageTitle = child.Name;
            }
            var tasks = child.GenerateTasks(library, directoryPath, System.IO.Path.GetFileNameWithoutExtension(child.Path), dataOpUnit, progressUpdatingAction);
            ret.AddRange(tasks);
            if (child is ImportPage)
            {
                var ip = child as ImportPage;
                ret.Add(new Task(() => library.AccessDispatcherObject(() => _book.AddPage(ip.GeneratedPage))));
            }
            ret.Add(new Task(() =>
            {
                ++Processed;
                progressUpdatingAction.Invoke(this, _book);
            }));
        }

        protected void SetDeliverables(ILibrary library)
        {
            library.AddToMemory(_book);
        }

        protected void GenerateDeliverables(DataOperationUnit dataOpUnit)
        {
            BookFacade.GetProeprty(ref _book, dataOpUnit);

            if (_book.FirstPage != null && !_book.FirstPage.Image.ThumbnailLoaded || !_book.FirstPage.Image.ThumbnailGenerated)
            {
                ThumbnailGenerating.GenerateThumbnail(_book.FirstPage.Image);
            }
        }

        private void Log()
        {
            s_logger.Info($"Imported [{AuthorName}]{Title}");
        }

        private void CreateTaskToInsertAuthor(string name, DataOperationUnit dataOpUnit)
        {
            var author = new AuthorViewModel(Guid.NewGuid(), name);
            author = AuthorFacade.InsertIfNotExists(author, dataOpUnit);
            _AuthorID = author.ID;
        }

        protected void CreateTaskToInsertBook(string entryName, string title, DataOperationUnit dataOpUnit)
        {
            _book = new BookViewModel(Guid.Parse(entryName), entryName);
            _book.Configuration = Configuration.ApplicationConfiguration;
            _book.Title = title;
            if (_AuthorID != Guid.Empty)
                _book.AuthorID = _AuthorID;

            BookFacade.Insert(_book, dataOpUnit);

            _children.ForEach(c => ((ImportPage)c).BookID = _book.ID);
            _BookID = _book.ID;
        }

        protected static void CreateTaskToCreateDirectoryIfDoesntExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                s_logger.Debug($"Create directory:{directoryPath}");
            }
        }
    }
}
