using Sunctum.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sunctum.Views
{
    /// <summary>
    /// LockIcon.xaml の相互作用ロジック
    /// </summary>
    public partial class LockIcon : UserControl
    {
        public LockIcon()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LockStateProperty = DependencyProperty.Register("LockState",
            typeof(LockState),
            typeof(LockIcon),
            new FrameworkPropertyMetadata(LockState.Collapsed));

        public LockState LockState
        {
            get { return (LockState)GetValue(LockStateProperty); }
            set { SetValue(LockStateProperty, value); }
        }
    }

    public enum LockState
    {
        Collapsed,
        Lock,
        Unlock,
    }
}
