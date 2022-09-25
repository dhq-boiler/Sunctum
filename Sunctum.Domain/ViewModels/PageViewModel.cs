

using Reactive.Bindings;
using Sunctum.Domain.Models;
using System;
using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public class PageViewModel : EntryViewModel
    {
        public PageViewModel()
        { }

        public PageViewModel(Guid id, string title)
            : base(id, title)
        { }

        public PageViewModel(Guid id, string title, int pageIndex)
            : base(id, title)
        {
            PageIndex = pageIndex;
        }

        private Guid _BookID;
        private Guid _ImageID;
        private int _PageIndex;

        private Configuration _Configuration;
        private ImageViewModel _Image;
        private bool _IsScrapped;
        private bool _ContentsRegistered;

        public Guid BookID
        {
            [DebuggerStepThrough]
            get
            { return _BookID; }
            set { SetProperty(ref _BookID, value); }
        }

        public Guid ImageID
        {
            [DebuggerStepThrough]
            get
            { return _ImageID; }
            set { SetProperty(ref _ImageID, value); }
        }

        public int PageIndex
        {
            [DebuggerStepThrough]
            get
            { return _PageIndex; }
            set { SetProperty(ref _PageIndex, value); }
        }

        public Configuration Configuration
        {
            [DebuggerStepThrough]
            get
            { return _Configuration; }
            set { SetProperty(ref _Configuration, value); }
        }

        public ImageViewModel Image
        {
            [DebuggerStepThrough]
            get
            { return _Image; }
            set { SetProperty(ref _Image, value); }
        }

        public bool IsScrapped
        {
            [DebuggerStepThrough]
            get
            { return _IsScrapped; }
            set { SetProperty(ref _IsScrapped, value); }
        }

        public bool ContentsRegisterd
        {
            [DebuggerStepThrough]
            get
            { return _ContentsRegistered; }
            set { SetProperty(ref _ContentsRegistered, value); }
        }
    }
}