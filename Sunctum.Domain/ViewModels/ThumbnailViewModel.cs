

using Sunctum.Domain.Models;
using System;

namespace Sunctum.Domain.ViewModels
{
    public class ThumbnailViewModel : NoPkIdEntityViewModel
    {
        public ThumbnailViewModel()
            : base()
        { }

        public ThumbnailViewModel(Guid id, Guid imageID, string path)
            : base()
        {
            ID = id;
            ImageID = imageID;
            RelativeMasterPath = path;
        }

        private Guid _ImageID;
        private string _RelativeMasterPath;

        public Guid ImageID
        {
            get { return _ImageID; }
            set { SetProperty(ref _ImageID, value); }
        }

        public string AbsoluteMasterPath
        {
            get
            {
                return $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{RelativeMasterPath}";
            }
        }

        public string RelativeMasterPath
        {
            get { return _RelativeMasterPath; }
            set { SetProperty(ref _RelativeMasterPath, value); }
        }

        public override string ToString()
        {
            return $"{{ImageID={ImageID}, AbsolutePath={AbsoluteMasterPath}}}";
        }
    }
}
