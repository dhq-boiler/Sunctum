

using Homura.ORM;
using Homura.ORM.Mapping;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [PrimaryKey("ImageID", "TagID")]
    [DefaultVersion(typeof(VersionOrigin))]
    public class ImageTag : EntityBaseObject
    {
        private Guid _imageID;
        private Guid _tagID;

        [Column("ImageID", "NUMERIC", 0)]
        [Since(typeof(VersionOrigin))]
        public Guid ImageID
        {
            [DebuggerStepThrough]
            get
            { return _imageID; }
            set { SetProperty(ref _imageID, value); }
        }

        [Column("TagID", "NUMERIC", 1)]
        [Since(typeof(VersionOrigin))]
        public Guid TagID
        {
            [DebuggerStepThrough]
            get
            { return _tagID; }
            set { SetProperty(ref _tagID, value); }
        }
    }
}
