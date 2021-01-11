

using PickoutCover.ViewModel;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Windows;

namespace PickoutCover.View.Dialog
{
    /// <summary>
    /// Interaction logic for PickOutCoverDialog.xaml
    /// </summary>
    public partial class PickOutCoverDialog : Window
    {
        public PickOutCoverDialogViewModel PocDialogVM { get; set; }

        public PickOutCoverDialog(PageViewModel page, ILibrary libraryVM)
        {
            InitializeComponent();
            DataContext = PocDialogVM = new PickOutCoverDialogViewModel(this, page, libraryVM);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PocDialogVM.Initialize();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            PocDialogVM.ExtractCover();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void LeftComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PocDialogVM.UpdateCoverSideBindingSource("RIGHT");
        }

        private void RightComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PocDialogVM.UpdateCoverSideBindingSource("LEFT");
        }

        private void BookSizePredictComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PocDialogVM == null) return;
            PocDialogVM.CoverLeftSide = null;
            PocDialogVM.CoverRightSide = null;
        }

        private void RadioButton_Edge_Left_Checked(object sender, RoutedEventArgs e)
        {
            if (PocDialogVM == null) return;
            PocDialogVM.SetLeftEdge();
        }

        private void RadioButton_Edge_Right_Checked(object sender, RoutedEventArgs e)
        {
            if (PocDialogVM == null) return;
            PocDialogVM.SetRightEdge();
        }

        private void RadioButton_Middle_Left_Checked(object sender, RoutedEventArgs e)
        {
            PocDialogVM.SetLeftMiddle();
        }

        private void RadioButton_Middle_Right_Checked(object sender, RoutedEventArgs e)
        {
            PocDialogVM.SetRightMiddle();
        }
    }
}
