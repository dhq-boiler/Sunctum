using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.IO;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class UnencryptionStarting : AsyncTaskMakerBase, IUnencryptionStarting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }
        public string Password { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start Unencryption"));
        }

        private EncryptImage _TargetEncryptImage;

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            var books = LibraryManager.Value.BookSource;

            foreach (var book in books)
            {
                ContentsLoadTask.FillContents(LibraryManager.Value, book);
                var images = book.Contents;
                foreach (var image in images)
                {
                    ContentsLoadTask.Load(image);
                    sequence.Add(() => _TargetEncryptImage = EncryptImageFacade.FindBy(image.Image.ID));
                    sequence.Add(() => Encryptor.Decrypt(_TargetEncryptImage.EncryptFilePath, image.Image.AbsoluteMasterPath, Password));
                    sequence.Add(() => File.Delete($"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{image.Image.ID}{Path.GetExtension(image.Image.AbsoluteMasterPath)}"));
                    sequence.Add(() => EncryptImageFacade.DeleteBy(image.Image.ID));
                }
            }
            sequence.Add(() => OnmemoryImageManager.Instance.Clear());
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = null);
            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = false);
        }
    }
}
