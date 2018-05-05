

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sunctum.Views
{
    /// <summary>
    /// ProgressBar.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        #region 依存プロパティ

        public static readonly DependencyProperty ProgressionRateProperty = DependencyProperty.Register("ProgressionRate",
            typeof(double),
            typeof(ProgressBar),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(ProgressBar.OnProgressionRateChanged), new CoerceValueCallback(ProgressBar.CoerceValuePercent)));

        public static readonly DependencyProperty InProcessBrushProperty = DependencyProperty.Register("InProcessBrush",
            typeof(Brush),
            typeof(ProgressBar),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xff, 0x75, 0x0)), new PropertyChangedCallback(ProgressBar.OnInProcessColor)));

        public static readonly DependencyProperty CompletedBrushProperty = DependencyProperty.Register("CompletedBrush",
            typeof(Brush),
            typeof(ProgressBar),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x0, 0x7a, 0xcc)), new PropertyChangedCallback(ProgressBar.OnCompletedBrush)));

        public static readonly DependencyProperty AbortedBrushProperty = DependencyProperty.Register("AbortedBrush",
            typeof(Brush),
            typeof(ProgressBar),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(ProgressBar.OnAbortedBrushChanged)));

        public static readonly DependencyProperty IsAbortedProperty = DependencyProperty.Register("IsAborted",
            typeof(bool),
            typeof(ProgressBar),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ProgressBar.OnIsAbortedChanged)));


        #endregion //依存プロパティ

        #region 依存プロパティコールバック

        private static void OnProgressionRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProgressBar;
            if (ctrl != null)
            {
                if ((double)e.NewValue < 1.0)
                    ctrl.Progress_Left_Grid.Background = ctrl.InProcessBrush;
                else
                    ctrl.Progress_Left_Grid.Background = ctrl.CompletedBrush;

                double total = ctrl.ActualWidth;
                double left = total * (double)e.NewValue;
                ctrl.Progress_Left_Grid.Width = left;
            }
        }

        private static void OnInProcessColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProgressBar;
            if (ctrl != null)
            {
            }
        }

        private static void OnCompletedBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProgressBar;
            if (ctrl != null)
            {
            }
        }

        private static void OnAbortedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProgressBar;
            if (ctrl != null)
            {
            }
        }

        private static void OnIsAbortedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProgressBar;
            if (ctrl != null)
            {
                if ((bool)e.NewValue)
                {
                    ctrl.Progress_Left_Grid.Background = ctrl.AbortedBrush;
                }
                else
                {
                    if (ctrl.ProgressionRate < 1.0)
                    {
                        ctrl.Progress_Left_Grid.Background = ctrl.InProcessBrush;
                    }
                    else
                    {
                        ctrl.Progress_Left_Grid.Background = ctrl.CompletedBrush;
                    }
                }
            }
        }

        #endregion //依存プロパティコールバック

        #region CLRプロパティ

        public double ProgressionRate
        {
            get { return (double)GetValue(ProgressionRateProperty); }
            set { SetValue(ProgressionRateProperty, value); }
        }

        public Brush InProcessBrush
        {
            get { return (Brush)GetValue(InProcessBrushProperty); }
            set { SetValue(InProcessBrushProperty, value); }
        }

        public Brush CompletedBrush
        {
            get { return (Brush)GetValue(CompletedBrushProperty); }
            set { SetValue(CompletedBrushProperty, value); }
        }

        public Brush AbortedBrush
        {
            get { return (Brush)GetValue(AbortedBrushProperty); }
            set { SetValue(AbortedBrushProperty, value); }
        }

        public bool IsAborted
        {
            get { return (bool)GetValue(IsAbortedProperty); }
            set { SetValue(IsAbortedProperty, value); }
        }

        #endregion

        #region Coerce Value Callback

        private static object CoerceValuePercent(DependencyObject d, object baseValue)
        {
            var ctrl = d as ProgressBar;
            double value = (double)baseValue;
            if (value < 0.0) value = 0.0;
            else if (value > 1.0) value = 1.0;
            return value;
        }

        #endregion // Coerce Value Callback

        private void Progress_DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double total = ActualWidth;
            double left = total * ProgressionRate;
            Progress_Left_Grid.Width = left;
        }
    }
}
