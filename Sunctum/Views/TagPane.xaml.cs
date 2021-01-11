

using Sunctum.Domain.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sunctum.Views
{
    /// <summary>
    /// TagPane.xaml の相互作用ロジック
    /// </summary>
    public partial class TagPane : UserControl
    {
        public TagPane()
        {
            InitializeComponent();
        }

        private Point? _tagListBox_MouseLeftButtonDown_Point;

        private void Tag_ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_tagListBox_MouseLeftButtonDown_Point.HasValue)
            {
                ListBox parent = (ListBox)sender;
                _tagListBox_MouseLeftButtonDown_Point = e.GetPosition(parent);
            }
        }

        private void Tag_ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tagListBox_MouseLeftButtonDown_Point.HasValue)
            {
                ListBox parent = (ListBox)sender;
                var data = GetDataFromListBox(parent, _tagListBox_MouseLeftButtonDown_Point.Value);

                if (data != null)
                {
                    DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
                    _tagListBox_MouseLeftButtonDown_Point = null;
                }
            }
        }

        private TagCountViewModel GetDataFromListBox(ListBox source, Point point)
        {
            IInputElement x = source.InputHitTest(point);

            if (x is FrameworkElement)
            {
                return ((FrameworkElement)x).DataContext as TagCountViewModel;
            }
            else if (x is FrameworkContentElement)
            {
                return ((FrameworkContentElement)x).DataContext as TagCountViewModel;
            }

            return null;
        }
    }
}
