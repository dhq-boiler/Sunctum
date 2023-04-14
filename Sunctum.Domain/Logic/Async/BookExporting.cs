using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                        sequence.Add(async () => await CopyFileWithTag(page));
                    }
                    else
                    {
                        sequence.Add(async () => await CopyFile(page));
                    }
                }
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookExporting"));
        }

        private async Task CopyFileWithTag(PageViewModel page)
        {
            var imageTag = ImageTagFacade.FindByImageId(page.ImageID);
            await CopyFile(page, imageTag.Select(a => a.Tag).ToArray());
        }

        private async Task CopyFiles(BookViewModel book, bool tag)
        {
            int pageCount = book.NumberOfPages.Value;
            for (int i = 0; i < pageCount; ++i)
            {
                var page = book[i];
                if (tag)
                {
                    var imageTag = ImageTagFacade.FindByImageId(page.ImageID);
                    await CopyFile(page, imageTag.Select(a => a.Tag).ToArray());
                }
                else
                {
                    await CopyFile(page);
                }
            }
        }

        private async Task CopyFile(PageViewModel page, params TagViewModel[] tags)
        {
            try
            {
                if (Configuration.ApplicationConfiguration.LibraryIsEncrypted)
                {
                    string tagsString = BuildTagsString(tags);
                    var encryptImage = EncryptImageFacade.FindBy(page.Image.ID);
                    await Encrypt.Encryptor.Decrypt(Configuration.ApplicationConfiguration.WorkingDirectory + encryptImage.EncryptFilePath, Configuration.ApplicationConfiguration.Password, false);
                    var decryptStream = OnmemoryImageManager.Instance.PullAsMemoryStream(page.Image.ID, false);
                    using (var writeTo = new FileStream($"{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}", FileMode.Create))
                    {
                        decryptStream.Seek(0, SeekOrigin.Begin);
                        decryptStream.CopyTo(writeTo);
                    }
                    s_logger.Debug($"FileCopy src:'{page.ImageID.ToString("N")}' dest:'{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}'");
                }
                else
                {
                    string tagsString = BuildTagsString(tags);
                    File.Copy(page.Image.AbsoluteMasterPath, $"{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}");
                    s_logger.Debug($"FileCopy src:'{page.Image.AbsoluteMasterPath}' dest:'{_bookDir}\\{Path.GetFileNameWithoutExtension(page.Image.AbsoluteMasterPath)}_{tagsString}{Path.GetExtension(page.Image.AbsoluteMasterPath)}'");
                }
            }
            catch (IOException e)
            {
                s_logger.Warn(e.Message);
            }
            catch (NullReferenceException e)
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

            if (book.Author is not null && !string.IsNullOrWhiteSpace(book.Author.Name))
            {
                _bookDir += $"[{book.Author?.Name}]";
            }
            _bookDir += escape(book.Title);

            CreateDirectoryIfDoesntExist(_bookDir);
        }

        private static string BuildTagsString(IEnumerable<TagViewModel> tags)
        {
            if (!tags.Any())
            {
                return string.Empty;
            }

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
