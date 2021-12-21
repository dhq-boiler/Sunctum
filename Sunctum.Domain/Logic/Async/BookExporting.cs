using NLog;
using Sunctum.Domain.Data.DaoFacade;
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
    public class BookExporting : AsyncTaskMakerBase, IBookExporting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private string _bookDir;

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        public IEnumerable<BookViewModel> TargetBooks { get; set; }

        public string DestinationDirectory { get; set; }

        public bool IncludeTag { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookExporting"));
            sequence.Add(() => s_logger.Info($"      To '{DestinationDirectory}'"));
            sequence.Add(() => s_logger.Info($"      IncludeTag : {IncludeTag}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => CreateDirectoryIfDoesntExist(DestinationDirectory));

            foreach (var book in TargetBooks)
            {
                LibraryManager.Value.RunFillContentsWithImage(book);
                sequence.Add(() => CreateDirectoryIfDoesntExist(DestinationDirectory, book, IncludeTag));
                foreach (var page in book.Contents)
                {
                    if (IncludeTag)
                    {
                        sequence.Add(() => CopyFileWithTag(page));
                    }
                    else
                    {
                        sequence.Add(() => CopyFile(page));
                    }
                }
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookExporting"));
        }

        private void CopyFileWithTag(PageViewModel page)
        {
            var imageTag = ImageTagFacade.FindByImageId(page.ImageID);
            CopyFile(page, imageTag.Select(a => a.Tag).ToArray());
        }

        private void CopyFiles(BookViewModel book, bool tag)
        {
            int pageCount = book.NumberOfPages.Value;
            for (int i = 0; i < pageCount; ++i)
            {
                var page = book[i];
                if (tag)
                {
                    var imageTag = ImageTagFacade.FindByImageId(page.ImageID);
                    CopyFile(page, imageTag.Select(a => a.Tag).ToArray());
                }
                else
                {
                    CopyFile(page);
                }
            }
        }

        private void CopyFile(PageViewModel page, params TagViewModel[] tags)
        {
            try
            {
                if (tags != null && tags.Count() > 0)
                {
                    string tagsString = BuildTagsString(tags);
                    File.Copy(page.Image.AbsoluteMasterPath, $"{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}");
                    s_logger.Debug($"FileCopy src:'{page.Image.AbsoluteMasterPath}' dest:'{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}'");
                }
                else
                {
                    File.Copy(page.Image.AbsoluteMasterPath, $"{_bookDir}\\{Path.GetFileName(page.Image.AbsoluteMasterPath)}");
                    s_logger.Debug($"FileCopy src:'{page.Image.AbsoluteMasterPath}' dest:'{_bookDir}\\{Path.GetFileName(page.Image.AbsoluteMasterPath)}'");
                }
            }
            catch (IOException e)
            {
                s_logger.Warn(e.Message);
            }
        }

        private void CreateDirectoryIfDoesntExist(string directory, BookViewModel book, bool tag)
        {
            _bookDir = $"{directory}\\";

            if (tag)
            {
                var imageIds = book.Contents.Select(a => a.Image.ID);
                var imageTags = ImageTagFacade.FindByImageIds(imageIds);
                var tags = ExtractTagsAllImagesAdded(imageTags);

                string tagString = BuildTagsString(tags);

                _bookDir += $"{tagString}";
            }

            _bookDir += $"[{book.Author?.Name}]{escape(book.Title)}";

            CreateDirectoryIfDoesntExist(_bookDir);
        }

        private static string BuildTagsString(IEnumerable<TagViewModel> tags)
        {
            string tagString = $"(";
            foreach (var t in tags)
            {
                tagString += t.Name;
                if (!tags.Last().Equals(t))
                {
                    tagString += ", ";
                }
            }
            tagString += ")";

            return tagString;
        }

        private static IEnumerable<TagViewModel> ExtractTagsAllImagesAdded(IEnumerable<ImageTagViewModel> imageTags)
        {
            var tags = imageTags.Select(a => a.TagID).Distinct();
            var images = imageTags.Select(a => a.ImageID).Distinct();

            foreach (var tag in tags)
            {
                if (imageTags.Count(a => a.TagID == tag) == images.Count())
                {
                    yield return imageTags.Where(a => a.TagID == tag).First().Tag;
                }
            }
        }

        private static void CreateDirectoryIfDoesntExist(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                s_logger.Debug($"Create directory:{directory}");
            }
        }

        private static string escape(string title)
        {
            return title.ReplaceAll(Path.GetInvalidPathChars(), '_').ReplaceAll(Path.GetInvalidFileNameChars(), '_');
        }
    }
}
