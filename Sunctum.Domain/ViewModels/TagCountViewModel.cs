

using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public class TagCountViewModel : BaseObjectViewModel
    {
        private TagViewModel _Tag;
        private int _Count;
        private bool _IsSearchingKey;
        private bool _IsVisible;

        public TagViewModel Tag
        {
            [DebuggerStepThrough]
            get
            { return _Tag; }
            set { SetProperty(ref _Tag, value); }
        }

        public int Count
        {
            [DebuggerStepThrough]
            get
            { return _Count; }
            set { SetProperty(ref _Count, value); }
        }

        public bool IsSearchingKey
        {
            [DebuggerStepThrough]
            get
            { return _IsSearchingKey; }
            set { SetProperty(ref _IsSearchingKey, value); }
        }

        public bool IsVisible
        {
            [DebuggerStepThrough]
            get
            { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        public TagCountViewModel()
        { }

        public TagCountViewModel(TagViewModel tag, int count)
        {
            Tag = tag;
            Count = count;
        }
    }
}
