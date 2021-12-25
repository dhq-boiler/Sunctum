using PickoutCover.ViewModels;
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
using Unity;

namespace PickoutCover.Views
{
    /// <summary>
    /// PickoutCover.xaml の相互作用ロジック
    /// </summary>
    public partial class PickoutCover : UserControl
    {
        [Dependency]
        public PickoutCoverViewModel PickoutCoverViewModel { get; set; }

        public PickoutCover()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PickoutCoverViewModel.Initialize();
        }

        private void LeftComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PickoutCoverViewModel.UpdateCoverSideBindingSource("RIGHT");
        }

        private void RightComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PickoutCoverViewModel.UpdateCoverSideBindingSource("LEFT");
        }

        private void BookSizePredictComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PickoutCoverViewModel == null) return;
            PickoutCoverViewModel.CoverLeftSide = null;
            PickoutCoverViewModel.CoverRightSide = null;
        }

        private void RadioButton_Edge_Left_Checked(object sender, RoutedEventArgs e)
        {
            if (PickoutCoverViewModel == null) return;
            PickoutCoverViewModel.SetLeftEdge();
        }

        private void RadioButton_Edge_Right_Checked(object sender, RoutedEventArgs e)
        {
            if (PickoutCoverViewModel == null) return;
            PickoutCoverViewModel.SetRightEdge();
        }

        private void RadioButton_Middle_Left_Checked(object sender, RoutedEventArgs e)
        {
            PickoutCoverViewModel.SetLeftMiddle();
        }

        private void RadioButton_Middle_Right_Checked(object sender, RoutedEventArgs e)
        {
            PickoutCoverViewModel.SetRightMiddle();
        }
    }
}
