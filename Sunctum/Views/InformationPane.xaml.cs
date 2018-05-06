

using Sunctum.Domain.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Sunctum.Views
{
    /// <summary>
    /// InformationPane.xaml の相互作用ロジック
    /// </summary>
    public partial class InformationPane : UserControl
    {
        public InformationPane()
        {
            InitializeComponent();
        }

        private void Information_ListView_SelectedEntriesTags_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }
    }
}
