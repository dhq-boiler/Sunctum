

using Homura.Core;
using NLog;
using Reactive.Bindings;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Sunctum.Domain.ViewModels
{
    public class ImageViewModel : EntryViewModel
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public ImageViewModel()
        { }

        public ImageViewModel(Guid id, string title, string masterPath, bool isEncrypted, Configuration config)
            : base(id, title)
        {
            Configuration = config;
            if (System.IO.Path.IsPathRooted(masterPath))
            {
                RelativeMasterPath = MakeRelativeMasterPath(Configuration.WorkingDirectory, masterPath);
            }
            else
            {
                RelativeMasterPath = masterPath;
            }
            IsEncrypted = isEncrypted;
        }

        public static string MakeRelativeMasterPath(string workingDirectory, string absoluteMasterPath)
        {
            Debug.Assert(absoluteMasterPath != null);
            Debug.Assert(System.IO.Path.IsPathRooted(absoluteMasterPath));

            Uri standard = new Uri(workingDirectory + "\\");
            Uri absolute = new Uri(absoluteMasterPath);
            Uri relative = standard.MakeRelativeUri(absolute);
            string relativePath = relative.ToString();
            relativePath = Uri.UnescapeDataString(relativePath);
            relativePath = relativePath.Replace("%25", "%");
            return relativePath;
        }

        private Configuration _Configuration;
        private string _RelativeMasterPath;
        private ThumbnailViewModel _Thumbnail;
        private long? _ByteSize;
        private bool _IsEncrypted;

        #region Configuration

        public Configuration Configuration
        {
            get { return _Configuration; }
            set { SetProperty(ref _Configuration, value); }
        }

        #endregion //Configuration

        public string OnStagePath
        {
            get
            {
                var encryptImage = EncryptImageFacade.FindBy(this.ID);
                if (encryptImage != null)
                {
                    if (string.IsNullOrEmpty(Configuration.Password))
                    {
                        return $"{Configuration.ApplicationConfiguration.ExecutingDirectory}\\{Specifications.LOCK_ICON_FILE}";
                    }
                    return encryptImage.EncryptFilePath;
                }

                return AbsoluteMasterPath;
            }
        }

        public string AbsoluteMasterPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RelativeMasterPath)) return null;
                return $"{Configuration.WorkingDirectory}\\{RelativeMasterPath}";
            }
        }

        public bool IsDecrypted
        {
            get
            {
                return OnmemoryImageManager.Instance.Exists(this.ID, false);
            }
        }

        public void DecryptImage(bool isThumbnail)
        {
            if (!OnmemoryImageManager.Instance.Exists(this.ID, isThumbnail))
            {
                var image = EncryptImageFacade.FindBy(this.ID);
                if (image is null)
                    return;
                try
                {
                    Encryptor.Decrypt(image.EncryptFilePath, Configuration.Password, isThumbnail);
                }
                catch (ArgumentException e)
                {
                    s_logger.Warn(e);
                }
            }
        }

        public string RelativeMasterPath
        {
            [DebuggerStepThrough]
            get
            { return _RelativeMasterPath; }
            set { SetProperty(ref _RelativeMasterPath, value); }
        }

        public long? ByteSize
        {
            [DebuggerStepThrough]
            get
            { return _ByteSize; }
            set { SetProperty(ref _ByteSize, value); }
        }

        public bool IsEncrypted
        {
            [DebuggerStepThrough]
            get
            { return _IsEncrypted; }
            set { SetProperty(ref _IsEncrypted, value); }
        }

        public ReactivePropertySlim<bool> TitleIsEncrypted { get; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<bool> TitleIsDecrypted { get; } = new ReactivePropertySlim<bool>();

        public ThumbnailViewModel Thumbnail
        {
            [DebuggerStepThrough]
            get
            { return _Thumbnail; }
            set
            {
                SetProperty(ref _Thumbnail, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => Path));
            }
        }

        public string Path
        {
            get
            {
                if (IsEncrypted)
                {
                    if (string.IsNullOrEmpty(Configuration.Password))
                    {
                        return $"{Configuration.ApplicationConfiguration.ExecutingDirectory}\\{Specifications.LOCK_ICON_FILE}";
                    }
                    return this.ID.ToString("D");
                }

                if (ThumbnailLoaded && !ThumbnailGenerated && MasterFileSize > Configuration.LowerLimitFileSizeThatImageMustBeDisplayAsThumbnail)
                {
                    var tg = new Logic.Async.ThumbnailGenerating();
                    tg.Target = this;
                    (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.RunSync(tg.GetTaskSequence());
                    return Thumbnail.AbsoluteMasterPath;
                }

                if (ThumbnailLoaded && ThumbnailGenerated)
                    return Thumbnail.AbsoluteMasterPath;
                else
                {
                    return AbsoluteMasterPath;
                }
            }
        }

        public long MasterFileSize
        {
            get
            {
                if (EncryptImageFacade.AnyEncrypted())
                {
                    var image = EncryptImageFacade.FindBy(this.ID);
                    Encryptor.Decrypt(image.EncryptFilePath, Configuration.Password, false);
                    return OnmemoryImageManager.Instance.PullAsMemoryStream(this.ID, false).Length;
                }

                if (File.Exists(AbsoluteMasterPath))
                {
                    return new FileInfo(AbsoluteMasterPath).Length;
                }
                else
                {
                    return 0L;
                }
            }
        }

        public bool ThumbnailRecorded
        {
            get
            {
                ThumbnailDao dao = new ThumbnailDao();
                return dao.CountBy(new Dictionary<string, object>() { { "ImageID", ID } }) > 0;
            }
        }

        public bool ThumbnailLoaded
        {
            get { return Thumbnail != null && Thumbnail.RelativeMasterPath != null; }
        }

        public bool ThumbnailGenerated
        {
            get
            {
                return Thumbnail?.AbsoluteMasterPath != null && File.Exists(Thumbnail.AbsoluteMasterPath);
            }
        }

        public override string ToString()
        {
            return "{RelativeMasterPath=" + RelativeMasterPath + ", ThumbnailGenerated=" + ThumbnailGenerated + ", Thumbnail=" + Thumbnail + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ImageViewModel i)
            {
                return ID.Equals(i.ID);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}