

using System.Windows;
using System.Windows.Controls;
using static Sunctum.UI.Core.Extensions;

namespace Sunctum.UI.Controls
{
    /// <summary>
    /// Interaction logic for SelectableLabel.xaml
    /// </summary>
    public partial class SelectableLabel : UserControl
    {
        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content",
            typeof(object),
            typeof(SelectableLabel),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SelectableLabel.OnContentChanged)));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoScrollingLabel ctrl = d as AutoScrollingLabel;
            if (ctrl != null)
            {
                ctrl.Control_Label.Content = ctrl.Content;
            }
        }

        public new object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public SelectableLabel()
        {
            InitializeComponent();
        }

        public Label TargetLabel
        {
            get { return Button_Label.GetVisualChild<Label>(); }
        }

        public TextBox TargetTextBox
        {
            get { return Button_Label.GetVisualChild<TextBox>(); }
        }

        private void Button_Label_Click(object sender, RoutedEventArgs e)
        {
            TargetLabel.Visibility = Visibility.Collapsed;
            TargetTextBox.Visibility = Visibility.Visible;
            TargetTextBox.Focus();
            TargetTextBox.SelectAll();
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            TargetLabel.Visibility = Visibility.Visible;
            TargetTextBox.Visibility = Visibility.Collapsed;
        }
    }
}
