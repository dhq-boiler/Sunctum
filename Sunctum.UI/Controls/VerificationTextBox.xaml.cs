﻿

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sunctum.UI.Controls
{
    /// <summary>
    /// Interaction logic for VerificationTextBox.xaml
    /// </summary>
    public partial class VerificationTextBox : UserControl
    {
        private DateTime _LastTextChanged;
        private Task _RequestUpdatingValidTask;

        #region ルーティングイベント

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VerificationTextBox));

        public static readonly RoutedEvent TextIsValidChangedEvent = EventManager.RegisterRoutedEvent("TextIsValidChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VerificationTextBox));

        #endregion //ルーティングイベント

        #region CLRイベント

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public event RoutedEventHandler TextIsValidChanged
        {
            add { AddHandler(TextIsValidChangedEvent, value); }
            remove { RemoveHandler(TextIsValidChangedEvent, value); }
        }

        #endregion //CLRイベント

        #region 依存プロパティ

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(VerificationTextBox),
            new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        public static readonly DependencyProperty TextIsValidProperty = DependencyProperty.Register("TextIsValid",
            typeof(bool?),
            typeof(VerificationTextBox));

        public static readonly DependencyProperty TextVerifierProperty = DependencyProperty.Register("TextVerifier", 
            typeof(Func<string, bool?>), 
            typeof(VerificationTextBox));

        #endregion //依存プロパティ

        #region CLRプロパティ

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool? TextIsValid
        {
            get { return (bool?)GetValue(TextIsValidProperty); }
            set { SetValue(TextIsValidProperty, value); }
        }

        public Func<string, bool?> TextVerifier
        {
            get { return (Func<string, bool?>)GetValue(TextVerifierProperty);}
            set { SetValue(TextVerifierProperty, value); }
        }

        #endregion //CLRプロパティ

        public VerificationTextBox()
        {
            InitializeComponent();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VerificationTextBox ctrl = d as VerificationTextBox;
            if (ctrl != null)
            {
                ctrl.TextBox.Text = ctrl.Text;
                ctrl.RequestUpdatingValid();
                ctrl.RaiseTextChangedEvent();
            }
        }

        private void RaiseTextChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(VerificationTextBox.TextChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void RaiseTextIsValidChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(VerificationTextBox.TextIsValidChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void RequestUpdatingValid()
        {
            _LastTextChanged = DateTime.Now;

            if (_RequestUpdatingValidTask != null) return;

            _RequestUpdatingValidTask = Task.Factory.StartNew(() => UpdateTextIsValid());
        }

        private void UpdateTextIsValid()
        {
            try
            {
                while ((DateTime.Now - _LastTextChanged).TotalMilliseconds < 1000)
                {
                    Thread.Sleep(100);
                }
                UpdateTextIsValidImmediately();
            }
            finally
            {
                _RequestUpdatingValidTask = null;
            }
        }

        public void UpdateTextIsValidImmediately()
        {
            Dispatcher.Invoke(() =>
            {
                if (TextVerifier != null)
                {
                    TextIsValid = Text != null && Text.Count() > 0 ? (bool?)TextVerifier(Text) : null;
                    RaiseTextIsValidChangedEvent();
                }
            });
        }
    }
}
