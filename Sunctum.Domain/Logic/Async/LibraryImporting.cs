

using Ninject;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Dao.Migration;
using Sunctum.Domain.Data.Rdbms;
using Sunctum.Domain.Logic.Generate;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryImporting : AsyncTaskMakerBase, ILibraryImporting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private static readonly string s_ANOTHER_DATABASE_ALIAS_NAME = "impt";

        private string _importLibraryDirectory;
        private SQLiteDataOperationUnit _dataOpUnit;
        private IEnumerable<PageViewModel> _pages;
        private IEnumerable<ImageViewModel> _images;

        [Inject]
        public ILibrary LibraryManager { get; set; }

        public string ImportLibraryFilename { get; set; }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            _importLibraryDirectory = Path.GetDirectoryName(ImportLibraryFilename);

            Initialize(ImportLibraryFilename);
            sequence.Add(new Task(() => CreateIDConversionTable()));
            sequence.Add(new Task(() => ImportAuthors(LibraryManager)));
            sequence.Add(new Task(() => ImportTags(LibraryManager)));
            sequence.AddRange(GenerateTasksToImportBooks(LibraryManager));
            sequence.Add(new Task(() => DropIDConversionTable()));
            sequence.Add(new Task(() => EndProcess()));
        }

        private void EndProcess()
        {
            _dataOpUnit.Dispose();
        }

        private void Initialize(string importLibraryFilename)
        {
            _dataOpUnit = new SQLiteDataOperationUnit();
            _dataOpUnit.Open(ConnectionManager.DefaultConnection);
            _dataOpUnit.AttachDatabase(importLibraryFilename, s_ANOTHER_DATABASE_ALIAS_NAME);
        }

        private IEnumerable<Task> GenerateTasksToImportBooks(ILibrary libManager)
        {
            List<Task> ret = new List<Task>();

            IDConversionDao idcDao = new IDConversionDao();
            BookDao bookDao = new BookDao();
            var anotherDbBooks = bookDao.FindAll(_dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToList().ToViewModel();
            var books = bookDao.FindAll(_dataOpUnit.CurrentConnection).ToList().ToViewModel();

            //同ID：対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
            var alpha = from b in anotherDbBooks
                        join ob in books on b.ID equals ob.ID
                        select b;

            foreach (var addBook in alpha)
            {
                var newGuid = Guid.NewGuid();
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Book", newGuid, addBook.ID), _dataOpUnit.CurrentConnection);
                addBook.ID = newGuid;

                ret.AddRange(GenerateTasksToImportBook(libManager, addBook));
            }

            //異ID：対象ライブラリのエンティティを変更せずそのままインポートする。
            var beta = from b in anotherDbBooks.Except(alpha)
                       select b;

            foreach (var addBook in beta)
            {
                ret.AddRange(GenerateTasksToImportBook(libManager, addBook));
            }

            return ret;
        }

        private IEnumerable<Task> GenerateTasksToImportBook(ILibrary libManager, BookViewModel addBook)
        {
            List<Task> ret = new List<Task>();

            ret.Add(new Task(() => ImportBook(libManager, addBook)));
            EnumeratePages(addBook);
            foreach (var addPage in _pages)
            {
                ret.AddRange(GenerateTasksToImportPage(libManager, addBook, addPage));
            }
            ret.Add(new Task(() => CopyImageTag(libManager, addBook)));
            ret.Add(new Task(() => libManager.AccessDispatcherObject(() => addBook.ContentsRegistered = true)));

            return ret;
        }

        private IEnumerable<Task> GenerateTasksToImportPage(ILibrary libManager, BookViewModel addBook, PageViewModel addPage)
        {
            List<Task> ret = new List<Task>();

            EnumerateImages(addPage);
            foreach (var addImage in _images)
            {
                ret.Add(new Task(() => ImportImage(addPage, addImage)));
            }
            ret.Add(new Task(() => ImportPage(libManager, addBook, addPage)));
            ret.Add(new Task(() => PrepareThumbnailIfFirstPage(libManager, addBook, addPage)));

            return ret;
        }

        private void EnumerateImages(PageViewModel parent)
        {
            ImageDao imageDao = new ImageDao();

            _images = imageDao.FindBy(new Dictionary<string, object>() { { "ID", parent.ImageID } }, _dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToViewModel();
        }

        private void EnumeratePages(BookViewModel parent)
        {
            PageDao pageDao = new PageDao();

            _pages = pageDao.FindBy(new Dictionary<string, object>() { { "BookID", parent.ID } }, _dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).OrderBy(p => p.PageIndex).ToViewModel();
        }

        private void CopyImageTag(ILibrary libManager, BookViewModel book)
        {
            IDConversionDao idcDao = new IDConversionDao();
            ImageTagDao itDao = new ImageTagDao();

            var imageTags = itDao.FindByBookId(s_ANOTHER_DATABASE_ALIAS_NAME, book.ID, _dataOpUnit.CurrentConnection).ToList().ToViewModel();

            foreach (var imageTag in imageTags)
            {
                var byImageId = idcDao.FindBy(new Dictionary<string, object>() { { "TableName", "Image" }, { "ForeignID", imageTag.ImageID } }, _dataOpUnit.CurrentConnection);
                var byTagId = idcDao.FindBy(new Dictionary<string, object>() { { "TableName", "Tag" }, { "ForeignID", imageTag.ImageID } }, _dataOpUnit.CurrentConnection);
                if (byImageId.Count() == 1)
                {
                    imageTag.ImageID = byImageId.Single().DomesticID;
                }
                if (byTagId.Count() == 1)
                {
                    imageTag.TagID = byTagId.Single().DomesticID;
                }
                itDao.Insert(imageTag.ToEntity(), _dataOpUnit.CurrentConnection);

                libManager.TagMng.Chains.Add(imageTag);
            }

            libManager.TagMng.ObserveTagCount();
        }

        private void CopyFile(ImageViewModel image, Guid bookId)
        {
            var sourceFilename = Path.Combine(_importLibraryDirectory, image.RelativeMasterPath);
            string newRelativeMasterPath = GenerateNewRelativeMasterPath(image, bookId);
            var destFilename = Path.Combine(Configuration.ApplicationConfiguration.WorkingDirectory, newRelativeMasterPath);
            var destDirectory = Path.GetDirectoryName(destFilename);

            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Copy(sourceFilename, destFilename);
        }

        private static string GenerateNewRelativeMasterPath(ImageViewModel image, Guid bookId)
        {
            Regex regex = new Regex("data/(?<year>\\d{4})/(?<month>\\d{2})/(?<day>\\d{2})/[a-z0-9]{32}/(?<filename>.+?)$");
            var match = regex.Match(image.RelativeMasterPath);
            var year = match.Groups["year"].Value;
            var month = match.Groups["month"].Value;
            var day = match.Groups["day"].Value;
            var filename = match.Groups["filename"].Value;
            var newRelativeMasterPath = $"data/{year}/{month}/{day}/{bookId.ToString("N")}/{filename}";
            return newRelativeMasterPath;
        }

        private void ImportImageWithContents(ILibrary libManager, PageViewModel parent)
        {
            ImageDao imageDao = new ImageDao();

            var images = imageDao.FindBy(new Dictionary<string, object>() { { "ID", parent.ImageID } }, _dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToViewModel();

            foreach (var image in images)
            {
                ImportImage(parent, image);
            }
        }

        private void ImportImage(PageViewModel parent, ImageViewModel image)
        {
            IDConversionDao idcDao = new IDConversionDao();
            ImageDao imageDao = new ImageDao();

            if (imageDao.FindBy(new Dictionary<string, object>() { { "ID", parent.ImageID } }, _dataOpUnit.CurrentConnection).Count() > 0)
            {
                //対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
                Guid newGuid = Guid.NewGuid();
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Image", newGuid, image.ID), _dataOpUnit.CurrentConnection);
                image.ID = newGuid;
            }

            CopyFile(image, parent.BookID);
            imageDao.Insert(image.ToEntity(), _dataOpUnit.CurrentConnection);
            parent.Image = image;
        }

        private void CopyPages(ILibrary libManager, BookViewModel parent)
        {
            IDConversionDao idcDao = new IDConversionDao();
            PageDao pageDao = new PageDao();

            var pages = pageDao.FindBy(new Dictionary<string, object>() { { "BookID", parent.ID } }, _dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).OrderBy(p => p.PageIndex).ToViewModel();

            foreach (var page in pages)
            {
                if (pageDao.FindBy(new Dictionary<string, object>() { { "ID", page.ID } }, _dataOpUnit.CurrentConnection).Count() > 0)
                {
                    //対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
                    Guid newGuid = Guid.NewGuid();
                    idcDao.Insert(new Data.Entity.Migration.IDConversion("Page", newGuid, page.ID), _dataOpUnit.CurrentConnection);
                    page.ID = newGuid;

                    ImportPageWithContents(libManager, parent, page);
                }
                else
                {
                    //対象ライブラリのエンティティを変更せずそのままインポートする。
                    ImportPageWithContents(libManager, parent, page);
                }
            }
        }

        private void ImportPageWithContents(ILibrary libManager, BookViewModel parent, PageViewModel page)
        {
            ImportImageWithContents(libManager, page);
            ImportPage(libManager, parent, page);
            PrepareThumbnailIfFirstPage(libManager, parent, page);
        }

        private void PrepareThumbnailIfFirstPage(ILibrary libManager, BookViewModel parent, PageViewModel page)
        {
            if (page.PageIndex == Specifications.PAGEINDEX_FIRSTPAGE)
            {
                ThumbnailGenerating.GenerateThumbnail(page.Image, _dataOpUnit);
                libManager.AccessDispatcherObject(() => parent.FirstPage = page);
                libManager.AccessDispatcherObject(() => parent.IsLoaded = true);
            }
        }

        private void ImportPage(ILibrary libManager, BookViewModel parent, PageViewModel page)
        {
            PageDao pageDao = new PageDao();
            pageDao.Insert(page.ToEntity(), _dataOpUnit.CurrentConnection);

            libManager.AccessDispatcherObject(() => parent.AddPage(page));
        }

        private void ImportBooks(ILibrary libManager)
        {
            IDConversionDao idcDao = new IDConversionDao();
            BookDao bookDao = new BookDao();
            var anotherDbBooks = bookDao.FindAll(_dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToList().ToViewModel();
            var books = bookDao.FindAll(_dataOpUnit.CurrentConnection).ToList().ToViewModel();

            //同ID：対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
            var alpha = from b in anotherDbBooks
                        join ob in books on b.ID equals ob.ID
                        select b;

            foreach (var add in alpha)
            {
                var newGuid = Guid.NewGuid();
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Book", newGuid, add.ID), _dataOpUnit.CurrentConnection);
                add.ID = newGuid;

                ImportBookWithContents(libManager, add);
            }

            //異ID：対象ライブラリのエンティティを変更せずそのままインポートする。
            var beta = from b in anotherDbBooks.Except(alpha)
                       select b;

            foreach (var add in beta)
            {
                ImportBookWithContents(libManager, add);
            }
        }

        private void ImportBookWithContents(ILibrary libManager, BookViewModel add)
        {
            ImportBook(libManager, add);
            CopyPages(libManager, add);
            CopyImageTag(libManager, add);
            libManager.AccessDispatcherObject(() => add.ContentsRegistered = true);
        }

        private void ImportBook(ILibrary libManager, BookViewModel add)
        {
            BookDao bookDao = new BookDao();

            if (add.AuthorID != null)
            {
                SetAuthorTo(libManager.AuthorManager, add);
            }
            bookDao.Insert(add.ToEntity(), _dataOpUnit.CurrentConnection);
            libManager.AddToMemory(add);
        }

        private void SetAuthorTo(IAuthorManager authorManager, BookViewModel add)
        {
            IDConversionDao idcDao = new IDConversionDao();
            AuthorDao authorDao = new AuthorDao();
            var idc = idcDao.FindBy(new Dictionary<string, object>() { { "TableName", "Author" }, { "ForeignID", add.AuthorID } }, _dataOpUnit.CurrentConnection);
            if (idc.Count() == 1)
            {
                add.Author = authorDao.FindBy(new Dictionary<string, object>() { { "ID", idc.Single().DomesticID } }, _dataOpUnit.CurrentConnection).SingleOrDefault().ToViewModel();
            }
            else
            {
                add.Author = authorDao.FindBy(new Dictionary<string, object>() { { "ID", add.AuthorID } }, _dataOpUnit.CurrentConnection).SingleOrDefault().ToViewModel();
            }
            authorManager.ObserveAuthorCount();
        }

        private void ImportTags(ILibrary libManager)
        {
            IDConversionDao idcDao = new IDConversionDao();
            TagDao tagDao = new TagDao();
            var anotherDbTags = tagDao.FindAll(_dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToViewModel();
            var tags = tagDao.FindAll(_dataOpUnit.CurrentConnection).ToViewModel();

            //同ID x 同値：対象ライブラリのエンティティをインポートしない。
            var alpha = from t in anotherDbTags
                        join ot in tags on t.ID equals ot.ID
                        where t.Name == ot.Name
                        select t;

            //同ID x 異値：対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
            var beta = from t in anotherDbTags.Except(alpha)
                       join ot in tags on t.ID equals ot.ID
                       where t.Name != ot.Name
                       select t;

            foreach (var add in beta)
            {
                var newGuid = Guid.NewGuid();
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Tag", newGuid, add.ID), _dataOpUnit.CurrentConnection);
                add.ID = newGuid;

                ImportTag(libManager, add);
            }

            //異ID x 同値：対象ライブラリのエンティティをインポートしない。取込側ライブラリに新規IDと旧IDを記録する。
            var gamma = from t in anotherDbTags.Except(alpha).Except(beta)
                        join ot in tags on t.Name equals ot.Name
                        where t.ID != ot.ID
                        select new { ForeignID = t.ID, DomesticID = ot.ID, Tag = t };

            foreach (var add in gamma)
            {
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Tag", add.DomesticID, add.ForeignID), _dataOpUnit.CurrentConnection);
            }

            //異ID x 異値：対象ライブラリのエンティティを変更せずそのままインポートする。
            var delta = from t in anotherDbTags.Except(alpha).Except(beta).Except(gamma.Select(t => t.Tag))
                        select t;

            foreach (var add in delta)
            {
                ImportTag(libManager, add);
            }
        }

        private void ImportTag(ILibrary libManager, TagViewModel add)
        {
            TagDao tagDao = new TagDao();
            tagDao.Insert(add.ToEntity(), _dataOpUnit.CurrentConnection);
            libManager.TagMng.Tags.Add(add);
        }

        private void ImportAuthors(ILibrary libManager)
        {
            IDConversionDao idcDao = new IDConversionDao();
            AuthorDao authorDao = new AuthorDao();
            var anotherDbAuthors = authorDao.FindAll(_dataOpUnit.CurrentConnection, s_ANOTHER_DATABASE_ALIAS_NAME).ToList().ToViewModel();
            var authors = authorDao.FindAll(_dataOpUnit.CurrentConnection).ToList().ToViewModel();

            //同ID x 同値：対象ライブラリのエンティティをインポートしない。
            var alpha = from a in anotherDbAuthors
                        join oa in authors on a.ID equals oa.ID
                        where a.Name == oa.Name
                        select a;

            //同ID x 異値：対象ライブラリのエンティティを新規IDでインポートする。取込側ライブラリに新規IDと旧IDを記録する。
            var beta = from a in anotherDbAuthors.Except(alpha)
                       join oa in authors on a.ID equals oa.ID
                       where a.Name != oa.Name
                       select a;

            foreach (var add in beta)
            {
                var newGuid = Guid.NewGuid();
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Author", newGuid, add.ID), _dataOpUnit.CurrentConnection);
                add.ID = newGuid;

                ImportAuthor(libManager, add);
            }

            //異ID x 同値：対象ライブラリのエンティティをインポートしない。取込側ライブラリに新規IDと旧IDを記録する。
            var gamma = from a in anotherDbAuthors.Except(alpha).Except(beta)
                        join oa in authors on a.Name equals oa.Name
                        where a.ID != oa.ID
                        select new { ForeignID = a.ID, DomesticID = oa.ID, Author = a };

            foreach (var add in gamma)
            {
                idcDao.Insert(new Data.Entity.Migration.IDConversion("Author", add.DomesticID, add.ForeignID), _dataOpUnit.CurrentConnection);
            }

            //異ID x 異値：対象ライブラリのエンティティを変更せずそのままインポートする。
            var delta = from a in anotherDbAuthors.Except(alpha).Except(beta).Except(gamma.Select(a => a.Author))
                        select a;

            foreach (var add in delta)
            {
                ImportAuthor(libManager, add);
            }
        }

        private void ImportAuthor(ILibrary libManager, AuthorViewModel add)
        {
            AuthorDao authorDao = new AuthorDao();
            authorDao.Insert(add.ToEntity(), _dataOpUnit.CurrentConnection);
            libManager.AuthorManager.Authors.Add(add);
        }

        private void CreateIDConversionTable()
        {
            s_logger.Info("Create IDConversion table...");

            try
            {
                IDConversionDao idConvertionDao = new IDConversionDao();
                idConvertionDao.CreateTableIfNotExists();

                s_logger.Info("Completed to create IDConversion table.");
            }
            catch (FailedOpeningDatabaseException)
            {
                throw;
            }
            catch (SqlException e)
            {
                s_logger.Error("Failed to create table.", e);
            }
        }

        private void DropIDConversionTable()
        {
            s_logger.Info("Drop IDConversion table...");

            try
            {
                IDConversionDao idConvertionDao = new IDConversionDao();
                idConvertionDao.DropTable();

                s_logger.Info("Completed to drop IDConversion table.");
            }
            catch (FailedOpeningDatabaseException)
            {
                throw;
            }
            catch (SqlException e)
            {
                s_logger.Error("Failed to drop table.", e);
            }
        }
    }
}
