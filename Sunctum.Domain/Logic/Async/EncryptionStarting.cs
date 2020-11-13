

using Ninject;
using NLog;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sunctum.Domain.Logic.Async
{
    public class EncryptionStarting : AsyncTaskMakerBase, IEncryptionStarting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public string Password { get; set; }

        [Inject]
        public ILibrary LibraryManager { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start Encryption"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            var books = LibraryManager.BookSource;

            sequence.Add(() => Configuration.ApplicationConfiguration.LibraryIsEncrypted = true);
            sequence.Add(() => Configuration.ApplicationConfiguration.Password = Password);
            sequence.Add(() => Directory.Delete($"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.CACHE_DIRECTORY}", true));

            foreach (var book in books)
            {
                ContentsLoadTask.FillContents(LibraryManager, book);
                var images = book.Contents;
                foreach (var image in images)
                {
                    ContentsLoadTask.Load(image);
                    sequence.Add(() => Encryptor.Encrypt(image.Image, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{image.Image.ID}{Path.GetExtension(image.Image.AbsoluteMasterPath)}", Password));
                    sequence.Add(() => Encryptor.DeleteOriginal(image));
                }
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish Encryption"));
        }
    }
}
