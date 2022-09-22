

using ChinhDo.Transactions;
using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace Sunctum.Domain.Logic.Import
{
    internal class ImportPage : Importer, IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private bool _isContent;
        private FileStream _fileStream;

        internal ImportPage(string path)
            : base(path)
        {
            if (Configuration.ApplicationConfiguration.LockFileInImporting)
            {
                _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                s_logger.Debug($"Lock:{path}");
            }
        }

        internal ImportPage(string path, bool isContent)
            : this(path)
        {
            _isContent = isContent;
        }

        public override string ToString()
        {
            return "Import File(Page) from " + Path;
        }

        public override void Estimate()
        {
            //Do nothing
        }

        public override IEnumerable<System.Threading.Tasks.Task> GenerateTasks(ILibrary library, string copyTo, string entryName, DataOperationUnit dataOpUnit, Action<Importer, BookViewModel> progressUpdatingAction)
        {
            List<System.Threading.Tasks.Task> ret = new List<System.Threading.Tasks.Task>();

            PageTitle = entryName;
            var filename = entryName + System.IO.Path.GetExtension(Path);
            var source = Path;
            Destination = copyTo + "\\" + filename;

            ret.Add(new System.Threading.Tasks.Task(() => CreateTaskToCopyImage(Path, source, Destination)));
            ret.Add(new System.Threading.Tasks.Task(() => CreateTaskToInsertImage(entryName, Destination, dataOpUnit)));
            if (_isContent)
            {
                ret.Add(new System.Threading.Tasks.Task(() => CreateTaskToInsertPage(entryName, dataOpUnit)));
            }
            ret.Add(new System.Threading.Tasks.Task(() => Dispose()));

            if (_isContent && Configuration.ApplicationConfiguration.LibraryIsEncrypted)
            {
                ret.Add(new System.Threading.Tasks.Task(() =>
                {
                    try
                    {
                        using (var scope = new TransactionScope())
                        {
                            var fileManager = new TxFileManager();
                            Encryptor.Encrypt(InsertedImage, $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.MASTER_DIRECTORY}\\{InsertedImage.ID.ToString().Substring(0, 2)}\\{InsertedImage.ID}{System.IO.Path.GetExtension(InsertedImage.AbsoluteMasterPath)}", Configuration.ApplicationConfiguration.Password, fileManager);
                            Encryptor.DeleteOriginal(GeneratedPage, fileManager);
                            InsertedImage.IsEncrypted = true;
                            ImageFacade.Update(InsertedImage);
                            scope.Complete();
                        }
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }));
            }

            return ret;
        }

        private void CreateTaskToInsertImage(string entryName, string destination, DataOperationUnit dataOpUnit)
        {
            Guid imageID = Guid.NewGuid();
            InsertedImage = new ImageViewModel(imageID, entryName, destination, Configuration.ApplicationConfiguration.LibraryIsEncrypted, Configuration.ApplicationConfiguration);
            InsertedImage.ByteSize = Size;
            ImageFacade.Insert(InsertedImage, dataOpUnit);
        }

        private void CreateTaskToInsertPage(string entryName, DataOperationUnit dataOpUnit)
        {
            Guid pageID = Guid.NewGuid();
            GeneratedPage = new PageViewModel(pageID, entryName);
            GeneratedPage.Configuration = Configuration.ApplicationConfiguration;
            GeneratedPage.ImageID = InsertedImage.ID;
            GeneratedPage.BookID = BookID;
            GeneratedPage.PageIndex = PageIndex;
            GeneratedPage.Image = InsertedImage;
            GeneratedPage.FingerPrint = FingerPrint = Hash.Generate(GeneratedPage);
            PageFacade.Insert(GeneratedPage, dataOpUnit);
        }

        private void CreateTaskToCopyImage(string Path, string source, string destination)
        {
            try
            {
                var fileInfo = new FileInfo(source);
                Size = fileInfo.Length;
                File.Copy(source, destination);
                s_logger.Debug($"Copy:{source} {FileSize.ConvertFileSizeUnit(Size)}");
            }
            catch (IOException e)
            {
                s_logger.Error($"Failed to copy:{Path}");
                s_logger.Debug(e);
                throw;
            }
        }

        public Guid BookID { get; set; }

        public ImageViewModel InsertedImage { get; private set; }

        public PageViewModel GeneratedPage { get; private set; }

        public int PageIndex { get; set; }

        public int TotalPageCount { get; set; }

        public string PageTitle { get; set; }

        public string Destination { get; private set; }
        public BookViewModel Book { get; internal set; }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                if (_fileStream != null)
                {
                    string path = _fileStream.Name;
                    _fileStream.Dispose();
                    s_logger.Debug($"Unlock:{path}");
                    _fileStream = null;
                }

                _disposedValue = true;
            }
        }

        ~ImportPage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
