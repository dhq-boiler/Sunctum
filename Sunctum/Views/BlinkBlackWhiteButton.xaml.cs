

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Sunctum.Views
{
    /*
     * https://social.msdn.microsoft.com/Forums/ja-JP/12618786-0ea2-4f22-8273-c044c3f7fb4d/usercontrolcontentusercontrol?forum=wpfja
     * みっと https://social.msdn.microsoft.com/profile/%E3%81%BF%E3%81%A3%E3%81%A8/?ws=usercard-mini
     * Microsoft Limited Public License
     */

    /// <summary>
    /// BlackWhiteButton.xaml の相互作用ロジック
    /// </summary>
    public partial class BlinkBlackWhiteButton : UserControl
    {
        public BlinkBlackWhiteButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register("ButtonContent", typeof(object), typeof(BlinkBlackWhiteButton), new PropertyMetadata(null));
        public static readonly DependencyProperty FocusAreaProperty = DependencyProperty.Register("FocusArea", typeof(FrameworkElement), typeof(BlinkBlackWhiteButton), new PropertyMetadata(null));

        public object ButtonContent
        {
            get { return GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public FrameworkElement FocusArea
        {
            get { return (FrameworkElement)GetValue(FocusAreaProperty); }
            set { SetValue(FocusAreaProperty, value); }
        }

        public event RoutedEventHandler Click
        {
            add { Button.Click += value; }
            remove { Button.Click -= value; }
        }

        public void Blink()
        {
            var storyboard = (Storyboard)FindResource("Storyboard_BlackWhite_Button_Opacity_Blink");
            var position = Mouse.GetPosition(FocusArea);
            if (VisualTreeHelper.HitTest(FocusArea, position) == null)
            {
                storyboard.Begin(Button);
                return;
            }
            storyboard = (Storyboard)FindResource("Storyboard_BlackWhite_Button_Color_Blink");
            storyboard.Begin(Button);
        }
    }
}
