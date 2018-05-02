

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Sunctum.UI.Controls
{
    /// <summary>
    /// Interaction logic for AutoScrollingHyperlink.xaml
    /// </summary>
    public partial class AutoScrollingHyperlink : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(AutoScrollingHyperlink),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(AutoScrollingHyperlink.OnTextChanged)));

        public static readonly RoutedEvent HyperlinkClickedEvent = EventManager.RegisterRoutedEvent(
            "HyperlinkClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutoScrollingHyperlink));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoScrollingHyperlink ctrl = d as AutoScrollingHyperlink;
            if (ctrl != null)
            {
                ctrl.Button_Hyperlink.Content = ctrl.Text;
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public event RoutedEventHandler HyperlinkClicked
        {
            add { AddHandler(HyperlinkClickedEvent, value); }
            remove { RemoveHandler(HyperlinkClickedEvent, value); }
        }

        private void RaiseHyperlinkClickedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AutoScrollingHyperlink.HyperlinkClickedEvent);
            RaiseEvent(newEventArgs);
        }

        public AutoScrollingHyperlink()
        {
            InitializeComponent();
        }

        public FrameworkElement Target
        {
            get { return Core.Extensions.GetVisualChild<TextBlock>(Button_Hyperlink); }
        }

        private BackgroundWorker _worker = new BackgroundWorker();
        private DateTime _beginAutoScroll;
        private DateTime _reachedDefaultPosition;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var labelWidth = Target.ActualWidth;
            if (labelWidth <= ActualWidth)
            {
                var space = ActualWidth - labelWidth;
                Dispatcher.Invoke(() =>
                {
                    Target.SetValue(Canvas.LeftProperty, space / 2d);
                });
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            RaiseHyperlinkClickedEvent();
        }

        public void EnableAutoScroll()
        {
            if (Target.ActualWidth <= ActualWidth || _worker.IsBusy)
            {
                return;
            }

            _worker.DoWork += Worker_DoWork;
            _worker.WorkerSupportsCancellation = true;
            _worker.RunWorkerAsync();
            _beginAutoScroll = DateTime.Now;
        }

        public void DisableAutoScroll()
        {
            if (_worker.WorkerSupportsCancellation)
            {
                _worker.CancelAsync();
                _worker.DoWork -= Worker_DoWork;
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_worker.CancellationPending)
            {
                if ((DateTime.Now - _beginAutoScroll).TotalMilliseconds < 1000)
                {
                    Thread.Sleep(10);
                    continue;
                }

                Dispatcher.Invoke(() =>
                {
                    if ((DateTime.Now - _reachedDefaultPosition).TotalMilliseconds < 2000)
                    {
                        return;
                    }

                    var labelWidth = Target.ActualWidth;
                    double left = (double)Target.GetValue(Canvas.LeftProperty);
                    if (left < -labelWidth)
                    {
                        left = ActualWidth + 0.5;
                    }

                    Target.SetValue(Canvas.LeftProperty, left - 0.5);

                    if (left - 0.5 == 0.0)
                    {
                        _reachedDefaultPosition = DateTime.Now;
                    }
                });
                Thread.Sleep(10);
            }

            Dispatcher.Invoke(() =>
            {
                Target.SetValue(Canvas.LeftProperty, 0d);
            });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DisableAutoScroll();
        }
    }
}
