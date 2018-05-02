

using System;
using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public class ImageTagViewModel : EntityBaseObjectViewModel
    {
        public ImageTagViewModel()
        { }

        public ImageTagViewModel(Guid imageID, TagViewModel tag)
        {
            ImageID = imageID;
            TagID = tag.ID;
            Tag = tag;
        }

        private Guid _imageID;
        private Guid _tagID;
        private TagViewModel _tag;

        public Guid ImageID
        {
            [DebuggerStepThrough]
            get
            { return _imageID; }
            set { SetProperty(ref _imageID, value); }
        }

        public Guid TagID
        {
            [DebuggerStepThrough]
            get
            { return _tagID; }
            set { SetProperty(ref _tagID, value); }
        }

        public TagViewModel Tag
        {
            [DebuggerStepThrough]
            get
            { return _tag; }
            set { SetProperty(ref _tag, value); }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null || obj.ToString().Equals("{DisconnectedItem}"))
                return false;
            var other = obj as ImageTagViewModel;
            return ImageID.Equals(other.ImageID) && Tag.Equals(other.Tag);
        }

        public override int GetHashCode()
        {
            return ImageID.GetHashCode() ^ Tag.GetHashCode();
        }
    }
}
