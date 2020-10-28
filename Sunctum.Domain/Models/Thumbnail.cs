

using Homura.ORM.Mapping;
using System;

namespace Sunctum.Domain.Models
{
    [PrimaryKey("ID", "ImageID")]
    [DefaultVersion(typeof(VersionOrigin))]
    public class Thumbnail : NoPkIdEntity
    {
        private Guid _ImageID;
        private string _RelativeMasterPath;

        [Column("ImageID", "NUMERIC", 1), Index]
        [Since(typeof(VersionOrigin))]
        public Guid ImageID
        {
            get { return _ImageID; }
            set { SetProperty(ref _ImageID, value); }
        }

        [Column("Path", "TEXT", 2), NotNull]
        [Since(typeof(VersionOrigin))]
        public string RelativeMasterPath
        {
            get { return _RelativeMasterPath; }
            set { SetProperty(ref _RelativeMasterPath, value); }
        }
    }
}
