

using NLog;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Controls;
using Sunctum.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Sunctum.Views
{
    /// <summary>
    /// HomeDocument.xaml の相互作用ロジック
    /// </summary>
    public partial class BookShowcase : UserControl
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public BookShowcase()
        {
            InitializeComponent();
        }

        private void Book_ListView_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void Book_ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer itemsViewer = (ScrollViewer)FindControl(Book_ListView, typeof(ScrollViewer));
            VirtualizingWrapPanel itemsPanel = (VirtualizingWrapPanel)FindControl(Book_ListView, typeof(VirtualizingWrapPanel));
            itemsPanel.Width = itemsViewer.ActualWidth;
        }

        private void Book_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            if (viewModel == null) return;

            viewModel.RemoveFromSelectedEntries(e.RemovedItems.Cast<EntryViewModel>());
            viewModel.MainWindowViewModel.LibraryVM.TagMng.Unselect(e.RemovedItems.Cast<EntryViewModel>());

            viewModel.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());
            viewModel.MainWindowViewModel.LibraryVM.TagMng.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());

            viewModel.MainWindowViewModel.LibraryVM.TagMng.ObserveSelectedEntityTags();
        }

        private void Book_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            object obj = item.Content;

            s_logger.Debug(obj as BookViewModel);

            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.OpenBook(obj as BookViewModel);
        }

        private void Book_ListViewItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            ProcessDragEventArgsInPreviewDragOver(ref e);
        }

        private void Book_ListViewItem_Drop(object sender, DragEventArgs e)
        {
            ProcessInDrop(sender, e);
        }

        private void Book_ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var ctrl = sender as ListViewItem;
            var label = UI.Core.Extensions.GetVisualChild<AutoScrollingLabel>(ctrl);
            label.EnableAutoScroll();

            var hyperlink = UI.Core.Extensions.GetVisualChild<AutoScrollingHyperlink>(ctrl);
            hyperlink.EnableAutoScroll();
        }

        private void Book_ListViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var ctrl = sender as ListViewItem;
            var label = UI.Core.Extensions.GetVisualChild<AutoScrollingLabel>(ctrl);
            label.DisableAutoScroll();

            var hyperlink = UI.Core.Extensions.GetVisualChild<AutoScrollingHyperlink>(ctrl);
            hyperlink.DisableAutoScroll();
        }

        // 最初に見つかったコントロールを返す http://pieceofnostalgy.blogspot.jp/2012/03/wpf-listboxitemspanelwrappanel.html
        private static DependencyObject FindControl(DependencyObject obj, Type controlType)
        {
            if (obj == null)
                return null;
            if (obj.GetType() == controlType)
                return obj;

            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                DependencyObject descendant = FindControl(child, controlType);
                if (descendant != null && descendant.GetType() == controlType)
                {
                    return descendant;
                }
            }

            return null;
        }

        private void Contents_ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer itemsViewer = (ScrollViewer)FindControl(Contents_ListView, typeof(ScrollViewer));
            WrapPanel itemsPanel = (WrapPanel)FindControl(Contents_ListView, typeof(WrapPanel));
            if (itemsPanel != null)
                itemsPanel.Width = itemsViewer.ActualWidth;
        }

        private void Contents_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            object obj = item.Content;

            s_logger.Debug(obj as PageViewModel);

            var str = obj.ToString();
            if (str.Equals("{DisconnectedItem}")) return;

            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.OpenImage((PageViewModel)obj);
        }

        private void Contents_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.ContentsListViewSelectedItems = Contents_ListView.SelectedItems.Cast<PageViewModel>().ToList();

            viewModel.RemoveFromSelectedEntries(e.RemovedItems.Cast<EntryViewModel>());
            viewModel.MainWindowViewModel.LibraryVM.TagMng.Unselect(e.RemovedItems.Cast<EntryViewModel>());

            viewModel.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());
            viewModel.MainWindowViewModel.LibraryVM.TagMng.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());

            viewModel.MainWindowViewModel.LibraryVM.TagMng.ObserveSelectedEntityTags();
        }

        private void Contents_ListViewItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            ProcessDragEventArgsInPreviewDragOver(ref e);
        }

        private void Contents_ListViewItem_Drop(object sender, DragEventArgs e)
        {
            ProcessInDrop(sender, e);
        }

        private void Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea_MouseEnter(object sender, MouseEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("Storyboard_BlackWhite_Button_Appear");
            storyboard.Begin(GoNext_Button);
            storyboard.Begin(GoBack_Button);
        }

        private void Grid_ImageViewer_Panel_BlackWhite_Button_MouseOverArea_MouseLeave(object sender, MouseEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("Storyboard_BlackWhite_Button_Disappear");
            storyboard.Begin(GoNext_Button);
            storyboard.Begin(GoBack_Button);
        }

        private void BackToBooks_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.CloseBook();
        }

        private void BackToPages_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.CloseImage();
        }

        private void GoBack_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.GoPreviousImage();
        }

        private void GoNext_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.GoNextImage();
        }

        private void MoveBackword_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.MovePageBackward((PageViewModel)(sender as Button).DataContext);
        }

        private void MoveForward_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.MovePageForward((PageViewModel)(sender as Button).DataContext);
        }

        internal bool SortingBookContents { get; set; }

        private async void SwitchSorting_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!SortingBookContents)
            {
                Contents_ListView.ItemTemplate = (DataTemplate)(this.Resources["Contents_ListViewItem_Sorting_DataTemplate"]);
                SwitchSorting_Button.Content = "Confirm";
                SortingBookContents = true;
            }
            else
            {
                Contents_ListView.ItemTemplate = (DataTemplate)(this.Resources["Contents_ListViewItem_DataTemplate"]);
                var viewModel = (DocumentViewModelBase)DataContext;
                await viewModel.SaveOpenedBookContentsOrder();
                SwitchSorting_Button.Content = "Sort";
                SortingBookContents = false;
            }
        }

        private void CloseSearchPane_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.CloseSearchPane();
        }

        private void Search_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.StoreScrollOffset(DocumentViewModelBase.BeforeSearchPosition);
            viewModel.Search();
            viewModel.ResetScrollOffset();
            e.Handled = true;
        }

        private void AutoScrollingHyperlink_HyperlinkClicked(object sender, RoutedEventArgs e)
        {
            AutoScrollingHyperlink hyperlink = (AutoScrollingHyperlink)sender;
            var author = (hyperlink.DataContext as BookViewModel).Author;
            var viewModel = (DocumentViewModelBase)DataContext;
            viewModel.StoreScrollOffset(DocumentViewModelBase.BeforeSearchPosition);
            viewModel.SearchText = author.Name;
            viewModel.ResetScrollOffset();
        }

        private static void ProcessDragEventArgsInPreviewDragOver(ref DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void ProcessInDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
            {
                var beDropped = (EntryViewModel)((ListViewItem)sender).DataContext;
                var viewModel = (DocumentViewModelBase)DataContext;
                var entries = viewModel.SelectedEntries.Contains(beDropped) ? viewModel.SelectedEntries.ToList() : new EntryViewModel[] { beDropped }.ToList();
                var imageTagCount = (TagCountViewModel)e.Data.GetData(typeof(TagCountViewModel));

                try
                {
                    viewModel.MainWindowViewModel.LibraryVM.TagMng.AddTagTo(entries, imageTagCount.Tag.Name);
                }
                catch (ArgumentException)
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }
    }
}
