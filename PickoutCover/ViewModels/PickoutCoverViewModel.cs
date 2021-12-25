

using Homura.Core;
using Homura.ORM;
using NLog;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using PickoutCover.Domain.Logic.Async;
using PickoutCover.Domain.Logic.CoverSegment;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Unity;

namespace PickoutCover.ViewModels
{
    public class PickoutCoverViewModel : NotifyPropertyChangedImpl, IDialogAware
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private const string CHANGE_COVER_SIDE_CANDIDATE_LEFT = "LEFT";
        private const string CHANGE_COVER_SIDE_CANDIDATE_RIGHT = "RIGHT";

        private CompositeDisposable disposables = new CompositeDisposable();
        private PageViewModel _page;
        private IEnumerable<int> _indexes;
        private CoverSideCandidate _coverLeftSide;
        private CoverSideCandidate _coverRightSide;
        private WriteableBitmap _baseBitmap;
        private WriteableBitmap _bitmap;
        private BookSize _SpecifiedBookSize;
        private CoverSideCandidate _selectedPredictedLeftSide;
        private CoverSideCandidate _selectedPredictedRightSide;
        private WriteableBitmap _preview;

        [Dependency]
        public ILibrary Library { get; set; }

        [Dependency]
        public ITaskManager TaskManager{ get; set; }

        public ReactiveCommand OKCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand CancelCommand { get; set; } = new ReactiveCommand();

        public PickoutCoverViewModel()
        {
            PropertyChanged += PickOutCoverDialogViewModel_PropertyChanged;
            OKCommand.Subscribe(_ =>
            {
                ExtractCover();
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            })
            .AddTo(disposables);
            CancelCommand.Subscribe(_ =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
            })
            .AddTo(disposables);
        }

        internal void ExtractCover()
        {
            Task.Factory.StartNew(async () =>
            {
                using (var trans = new DataOperationUnit())
                {
                    trans.Open(ConnectionManager.DefaultConnection);
                    var coverSegmenting = new CoverSegmenting(Library, _page, CoverLeftSide, CoverRightSide, trans);
                    await TaskManager.Enqueue(coverSegmenting.GetTaskSequence());
                }
            });
        }

        public WriteableBitmap PreviewBitmap
        {
            get { return _preview; }
            set
            {
                SetProperty(ref _preview, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => PreviewAvailables));
            }
        }

        public bool PreviewAvailables
        {
            get { return PreviewBitmap != null; }
        }

        internal void SetRightEdge()
        {
            CoverRightSide = new CoverSideCandidate(BitmapWidth - 1, false);
        }

        internal void SetLeftEdge()
        {
            CoverLeftSide = new CoverSideCandidate(0, false);
        }

        internal void SetRightMiddle()
        {
            CoverRightSide = new CoverSideCandidate(BitmapWidth / 2, false);
        }

        internal void SetLeftMiddle()
        {
            CoverLeftSide = new CoverSideCandidate(BitmapWidth / 2, false);
        }

        private void UpdateBitmap()
        {
            var temp = _baseBitmap.Clone();
            int width = temp.PixelWidth;
            if (CoverLeftSide == null && CoverRightSide == null)
            {
                DrawAll(temp);
            }
            else if ((CoverLeftSide != null && CoverLeftSide.Offset >= width)
                || (CoverRightSide != null && CoverRightSide.Offset >= width)
                || (CoverLeftSide != null && CoverRightSide != null && CoverLeftSide.Offset >= CoverRightSide.Offset))
            {
                DrawGrayAll(temp);
            }
            else if (CoverLeftSide != null && CoverLeftSide.Offset < width
                && CoverRightSide != null && CoverRightSide.Offset < width
                && CoverLeftSide.Offset < CoverRightSide.Offset)
            {
                DrawLeftRight(temp);
            }
            else if (CoverLeftSide != null && CoverLeftSide.Offset < width)
            {
                DrawLeft(temp);
            }
            else if (CoverRightSide != null && CoverRightSide.Offset < width)
            {
                DrawRight(temp);
            }
            Bitmap = temp;
        }

        private void DrawGrayAll(WriteableBitmap temp)
        {
            CoverSegmentExtractor.GrayoutLeft(temp, temp.PixelWidth);
        }

        private void DrawAll(WriteableBitmap temp)
        {
            CoverSegmentExtractor.Line(temp, SegmentCandidateIndexes, Color.FromArgb(128, 255, 0, 0));
        }

        private void DrawRight(WriteableBitmap temp)
        {
            CoverSegmentExtractor.GrayoutRight(temp, CoverRightSide.Offset);
            CoverSegmentExtractor.BoldLine(temp, new int[] { CoverRightSide.Offset }, Color.FromArgb(128, 255, 0, 0));
            CoverSegmentExtractor.Line(temp, SegmentCandidateIndexes.Where(a => a < CoverRightSide.Offset), Color.FromArgb(128, 255, 0, 0));
        }

        private void DrawLeft(WriteableBitmap temp)
        {
            CoverSegmentExtractor.GrayoutLeft(temp, CoverLeftSide.Offset);
            CoverSegmentExtractor.BoldLine(temp, new int[] { CoverLeftSide.Offset }, Color.FromArgb(128, 255, 0, 0));
            CoverSegmentExtractor.Line(temp, SegmentCandidateIndexes.Where(a => a > CoverLeftSide.Offset), Color.FromArgb(128, 255, 0, 0));
        }

        private void DrawLeftRight(WriteableBitmap temp)
        {
            CoverSegmentExtractor.GrayoutLeft(temp, CoverLeftSide.Offset);
            CoverSegmentExtractor.GrayoutRight(temp, CoverRightSide.Offset);
            CoverSegmentExtractor.BoldLine(temp, new int[] { CoverLeftSide.Offset, CoverRightSide.Offset }, Color.FromArgb(128, 255, 0, 0));
        }

        private void PickOutCoverDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CoverLeftSide":
                case "CoverRightSide":
                    UpdateBitmap();
                    if (IsValid)
                    {
                        UpdatePreviewBitmap();
                    }
                    break;
            }

            switch (e.PropertyName)
            {
                case "CoverLeftSide":
                    s_logger.Debug($"CoverLeftSide:{CoverLeftSide}");
                    break;
                case "CoverRightSide":
                    s_logger.Debug($"CoverRightSide:{CoverRightSide}");
                    break;
            }
        }

        private void UpdatePreviewBitmap()
        {
            using (Mat mat = new Mat(_page.Image.AbsoluteMasterPath, ImreadModes.Unchanged))
            using (Mat ext = CoverSegmenting.ExtractRect(mat, new OpenCvSharp.Rect(CoverLeftSide.Offset, 0, CoverRightSide.Offset - CoverLeftSide.Offset + 1, mat.Height)))
            {
                PreviewBitmap = WriteableBitmapConverter.ToWriteableBitmap(ext);
            }
        }

        public WriteableBitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                SetProperty(ref _bitmap, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => BitmapWidth),
                                  PropertyNameUtility.GetPropertyName(() => BitmapHeight));
            }
        }

        public int BitmapWidth
        {
            get { return Bitmap != null ? Bitmap.PixelWidth : -1; }
        }

        public int BitmapHeight
        {
            get { return Bitmap != null ? Bitmap.PixelHeight : -1; }
        }

        public IEnumerable<int> SegmentCandidateIndexes
        {
            get { return _indexes; }
            set
            {
                SetProperty(ref _indexes, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => CoverLeftSideSource),
                                  PropertyNameUtility.GetPropertyName(() => CoverRightSideSource));
            }
        }

        private IEnumerable<CoverSideCandidate> _CoverLeftSideSource;
        private IEnumerable<CoverSideCandidate> _CoverRightSideSource;

        public event Action<IDialogResult> RequestClose;

        public IEnumerable<CoverSideCandidate> CoverLeftSideSource
        {
            get { return _CoverLeftSideSource; }
            set { SetProperty(ref _CoverLeftSideSource, value); }
        }

        public IEnumerable<CoverSideCandidate> CoverRightSideSource
        {
            get { return _CoverRightSideSource; }
            set { SetProperty(ref _CoverRightSideSource, value); }
        }

        public CoverSideCandidate CoverLeftSide
        {
            get { return _coverLeftSide; }
            set
            {
                if (value != null)
                {
                    _selectedPredictedLeftSide = value;
                }
                else if (value != null)
                {
                    _selectedPredictedLeftSide = null;
                }
                SetProperty(ref _coverLeftSide, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => IsValid),
                                  PropertyNameUtility.GetPropertyName(() => PreviewBitmap));
            }
        }

        public CoverSideCandidate CoverRightSide
        {
            get { return _coverRightSide; }
            set
            {
                if (value != null)
                {
                    _selectedPredictedRightSide = value;
                }
                else if (value != null)
                {
                    _selectedPredictedRightSide = null;
                }
                SetProperty(ref _coverRightSide, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => IsValid),
                                  PropertyNameUtility.GetPropertyName(() => PreviewBitmap));
            }
        }

        internal void Initialize()
        {
            SegmentCandidateIndexes = CoverSegmentExtractor.ExtractCoverVerticalSegmentIndexes(_page.Image.AbsoluteMasterPath);
            UpdateBitmap();
            UpdateCoverSideBindingSource(CHANGE_COVER_SIDE_CANDIDATE_LEFT);
            UpdateCoverSideBindingSource(CHANGE_COVER_SIDE_CANDIDATE_RIGHT);
        }

        public bool IsValid
        {
            get { return CoverLeftSide != null && CoverRightSide != null && CoverLeftSide.Offset < CoverRightSide.Offset && CoverRightSide.Offset < Bitmap.PixelWidth; }
        }

        public IEnumerable<BookSize> BookSizes
        {
            get
            {
                return new List<BookSize>()
                {
                    new BookSize("B4判", 257, 364),
                    new BookSize("A4判", 210, 297),
                    new BookSize("B5判", 182, 257),
                    new BookSize("A5判", 148, 210),
                    new BookSize("B6判", 128, 182),
                    new BookSize("A6判", 105, 148),
                    new BookSize("菊判", 150, 220),
                    new BookSize("四六判", 127, 188),
                    new BookSize("AB判", 210, 257),
                    new BookSize("小B6判", 112, 174),
                    new BookSize("三五判", 84, 148),
                    new BookSize("新書判", 103, 182),
                    new BookSize("重箱判", 182, 206),
                    new BookSize("タブロイド判", 273, 406),
                    new BookSize("ブランケット判", 406, 546)
                };
            }
        }

        public BookSize SpecifiedBookSize
        {
            get { return _SpecifiedBookSize; }
            set
            {
                SetProperty(ref _SpecifiedBookSize, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => CoverLeftSideSource),
                                  PropertyNameUtility.GetPropertyName(() => CoverRightSideSource));
            }
        }

        public string Title => throw new NotImplementedException();

        public void UpdateCoverSideBindingSource(string place, bool keepSelectedItem = true)
        {
            switch (place)
            {
                case CHANGE_COVER_SIDE_CANDIDATE_LEFT:
                    if (CoverRightSide != null)
                    {
                        CoverLeftSideSource = CreateCoverSideBindingSource(SegmentCandidateIndexes.Where(a => a < CoverRightSide.Offset), CHANGE_COVER_SIDE_CANDIDATE_LEFT);
                    }
                    else if (CoverLeftSide == null)
                    {
                        CoverLeftSideSource = CreateCoverSideBindingSource(SegmentCandidateIndexes, CHANGE_COVER_SIDE_CANDIDATE_LEFT);
                    }
                    break;
                case CHANGE_COVER_SIDE_CANDIDATE_RIGHT:
                    if (CoverLeftSide != null)
                    {
                        CoverRightSideSource = CreateCoverSideBindingSource(SegmentCandidateIndexes.Where(a => a > CoverLeftSide.Offset), CHANGE_COVER_SIDE_CANDIDATE_RIGHT);
                    }
                    else if (CoverRightSide == null)
                    {
                        CoverRightSideSource = CreateCoverSideBindingSource(SegmentCandidateIndexes, CHANGE_COVER_SIDE_CANDIDATE_RIGHT);
                    }
                    break;
            }
        }

        private IEnumerable<CoverSideCandidate> CreateCoverSideBindingSource(IEnumerable<int> source, string place)
        {
            List<CoverSideCandidate> ret = new List<CoverSideCandidate>(source.Select(a => new CoverSideCandidate(a, false)));

            if (SpecifiedBookSize != null)
            {
                switch (place)
                {
                    case CHANGE_COVER_SIDE_CANDIDATE_LEFT:
                        if (CoverLeftSide == null && CoverRightSide != null && !CoverRightSide.IsPredicted)
                        {
                            var candidate = CoverRightSide.Offset - SpecifiedBookSize.Pixels.Width * Bitmap.PixelHeight / SpecifiedBookSize.Pixels.Height;
                            if (ret.Where(a => a.Offset == candidate).Count() > 0)
                            {
                                var existed = ret.Where(a => a.Offset == candidate).Single();
                                existed.IsPredicted = true;
                            }
                            else if (candidate >= 0)
                            {
                                ret.Add(new CoverSideCandidate((int)candidate, true));
                            }
                        }
                        if (CoverLeftSide != null && CoverRightSide != null && _selectedPredictedLeftSide != null)
                        {
                            if (source.Where(a => a == _selectedPredictedLeftSide.Offset).Count() == 0)
                            {
                                ret.Add(_selectedPredictedLeftSide);
                            }
                            else
                            {
                                var willRemoves = ret.Where(a => a.Offset == _selectedPredictedLeftSide.Offset);
                                for (int i = 0; i < willRemoves.Count(); ++i)
                                {
                                    ret.Remove(willRemoves.ElementAt(i));
                                }
                                ret.Add(_selectedPredictedLeftSide);
                            }
                        }
                        break;
                    case CHANGE_COVER_SIDE_CANDIDATE_RIGHT:
                        if (CoverLeftSide != null && CoverRightSide == null && !CoverLeftSide.IsPredicted)
                        {
                            var candidate = CoverLeftSide.Offset + SpecifiedBookSize.Pixels.Width * Bitmap.PixelHeight / SpecifiedBookSize.Pixels.Height;
                            if (ret.Where(a => a.Offset == candidate).Count() > 0)
                            {
                                var existed = ret.Where(a => a.Offset == candidate).Single();
                                existed.IsPredicted = true;
                            }
                            else if (candidate >= 0)
                            {
                                ret.Add(new CoverSideCandidate((int)candidate, true));
                            }
                        }
                        if (CoverLeftSide != null && CoverRightSide != null && _selectedPredictedRightSide != null)
                        {
                            if (source.Where(a => a == _selectedPredictedRightSide.Offset).Count() == 0)
                            {
                                ret.Add(_selectedPredictedRightSide);
                            }
                            else
                            {
                                var willRemoves = ret.Where(a => a.Offset == _selectedPredictedRightSide.Offset);
                                for (int i = 0; i < willRemoves.Count(); ++i)
                                {
                                    ret.Remove(willRemoves.ElementAt(i));
                                }
                                ret.Add(_selectedPredictedRightSide);
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (place)
                {
                    case CHANGE_COVER_SIDE_CANDIDATE_LEFT:
                        if (_selectedPredictedLeftSide != null)
                        {
                            if (source.Where(a => a == _selectedPredictedLeftSide.Offset).Count() == 0)
                            {
                                ret.Add(_selectedPredictedLeftSide);
                            }
                            else
                            {
                                var willRemoves = ret.Where(a => a.Offset == _selectedPredictedLeftSide.Offset);
                                for (int i = 0; i < willRemoves.Count(); ++i)
                                {
                                    ret.Remove(willRemoves.ElementAt(i));
                                }
                                ret.Add(_selectedPredictedLeftSide);
                            }
                        }
                        break;
                    case CHANGE_COVER_SIDE_CANDIDATE_RIGHT:
                        if (_selectedPredictedRightSide != null)
                        {
                            if (source.Where(a => a == _selectedPredictedRightSide.Offset).Count() == 0)
                            {
                                ret.Add(_selectedPredictedRightSide);
                            }
                            else
                            {
                                var willRemoves = ret.Where(a => a.Offset == _selectedPredictedRightSide.Offset);
                                for (int i = 0; i < willRemoves.Count(); ++i)
                                {
                                    ret.Remove(willRemoves.ElementAt(i));
                                }
                                ret.Add(_selectedPredictedRightSide);
                            }
                        }
                        break;
                }
            }

            return ret.OrderBy(a => a.Offset);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _page = parameters.GetValue<PageViewModel>("page");
            _baseBitmap = CoverSegmentExtractor.LoadBitmap(_page.Image.AbsoluteMasterPath);
            Initialize();
        }

        public class BookSize : BaseObject
        {
            private string _Name;
            private System.Windows.Size _Millimeters;
            public static readonly double MILLIMETERS_PER_INCH = 25.4;
            public static readonly int DPI = 96;

            public string Name
            {
                get { return _Name; }
                set { SetProperty(ref _Name, value); }
            }

            public System.Windows.Size Millimeters
            {
                get { return _Millimeters; }
                set { SetProperty(ref _Millimeters, value); }
            }

            public BookSize(string name, System.Windows.Size millimeters)
            {
                Name = name;
                Millimeters = millimeters;
            }

            public BookSize(string name, double width_mm, double height_mm)
            {
                Name = name;
                Millimeters = new System.Windows.Size(width_mm, height_mm);
            }

            public System.Windows.Size Pixels
            {
                get
                {
                    return new System.Windows.Size(Math.Round(Millimeters.Width * DPI / MILLIMETERS_PER_INCH),
                        Math.Round(Millimeters.Height * DPI / MILLIMETERS_PER_INCH));
                }
            }
        }
    }
}
