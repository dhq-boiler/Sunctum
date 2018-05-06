

using Ninject;
using NLog;
using Sunctum.Domain.Models;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        [Inject]
        public IMainWindowViewModel MainWindowVM { get; set; }

        internal bool SortingBookContents { get; set; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainWindowVM;
            bool ShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            await MainWindowVM.Initialize(true, ShiftPressed);
        }

        private async void Book_ListView_Drop(object sender, DragEventArgs e)
        {
            string[] objects = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (objects != null)
            {
                await MainWindowVM.ImportAsync(objects);
            }
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

        private void Book_ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            object obj = item.Content;

            s_logger.Debug(obj as BookViewModel);

            MainWindowVM.OpenBook(obj as BookViewModel);
        }

        private void Book_ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer itemsViewer = (ScrollViewer)FindControl(Book_ListView, typeof(ScrollViewer));
            VirtualizingWrapPanel itemsPanel = (VirtualizingWrapPanel)FindControl(Book_ListView, typeof(VirtualizingWrapPanel));
            itemsPanel.Width = itemsViewer.ActualWidth;
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
            MainWindowVM.OpenImage((PageViewModel)obj);
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
            MainWindowVM.CloseBook();
        }

        private void BackToPages_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.CloseImage();
        }

        private void GoBack_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.GoPreviousImage();
        }

        private void GoNext_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.GoNextImage();
        }

        private void MoveBackword_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.MovePageBackward((PageViewModel)(sender as Button).DataContext);
        }

        private void MoveForward_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.MovePageForward((PageViewModel)(sender as Button).DataContext);
        }

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
                await MainWindowVM.SaveOpenedBookContentsOrder();
                SwitchSorting_Button.Content = "Sort";
                SortingBookContents = false;
            }
        }

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainWindowVM.ResetScrollOffsetPool();
        }

        private void CloseSearchPane_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.CloseSearchPane();
        }

        private void Search_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindowVM.Search();
            e.Handled = true;
        }

        private void Book_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.BookListViewSelectedItems = Book_ListView.SelectedItems.Cast<BookViewModel>().ToList();

            MainWindowVM.RemoveFromSelectedEntries(e.RemovedItems.Cast<EntryViewModel>());
            MainWindowVM.LibraryVM.TagMng.Unselect(e.RemovedItems.Cast<EntryViewModel>());

            MainWindowVM.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());
            MainWindowVM.LibraryVM.TagMng.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());

            MainWindowVM.LibraryVM.TagMng.ObserveSelectedEntityTags();
        }

        private void Contents_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.ContentsListViewSelectedItems = Contents_ListView.SelectedItems.Cast<PageViewModel>().ToList();

            MainWindowVM.RemoveFromSelectedEntries(e.RemovedItems.Cast<EntryViewModel>());
            MainWindowVM.LibraryVM.TagMng.Unselect(e.RemovedItems.Cast<EntryViewModel>());

            MainWindowVM.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());
            MainWindowVM.LibraryVM.TagMng.AddToSelectedEntries(e.AddedItems.Cast<EntryViewModel>());

            MainWindowVM.LibraryVM.TagMng.ObserveSelectedEntityTags();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindowVM.Terminate();
        }

        private void Book_ListViewItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            ProcessDragEventArgsInPreviewDragOver(ref e);
        }

        private void Book_ListViewItem_Drop(object sender, DragEventArgs e)
        {
            ProcessInDrop(sender, MainWindowVM, e);
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

        private void AutoScrollingHyperlink_HyperlinkClicked(object sender, RoutedEventArgs e)
        {
            AutoScrollingHyperlink hyperlink = (AutoScrollingHyperlink)sender;
            var author = hyperlink.DataContext as AuthorViewModel;
            MainWindowVM.SearchText = author.Name;
        }

        private void Contents_ListViewItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            ProcessDragEventArgsInPreviewDragOver(ref e);
        }

        private void Contents_ListViewItem_Drop(object sender, DragEventArgs e)
        {
            ProcessInDrop(sender, MainWindowVM, e);
        }

        private static void ProcessDragEventArgsInPreviewDragOver(ref DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private static void ProcessInDrop(object sender, IMainWindowViewModel MainWindowVM, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCount)))
            {
                var beDropped = (EntryViewModel)((ListViewItem)sender).DataContext;
                var entries = MainWindowVM.SelectedEntries.Contains(beDropped) ? MainWindowVM.SelectedEntries.ToList() : new EntryViewModel[] { beDropped }.ToList();
                var imageTagCount = (TagCountViewModel)e.Data.GetData(typeof(TagCountViewModel));

                try
                {
                    MainWindowVM.LibraryVM.TagMng.AddTagTo(entries, imageTagCount.Tag.Name);
                }
                catch (ArgumentException)
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var author = ((FrameworkContentElement)sender).DataContext as Author;
            MainWindowVM.SearchText = author.Name;
        }

        #region Tagペイン

        private void Tag_Button_Close_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.DisplayTagPane = false;
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

        private async void Tag_ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listbox = (ListBox)sender;

            if (listbox.SelectedItems.Count == 0)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Delete:
                    await MainWindowVM.LibraryVM.TagMng.RemoveTag(listbox.SelectedItems.Cast<TagCountViewModel>().Select(a => a.Tag.Name));
                    break;
                default:
                    break;
            }
        }

        private void Button_TagPane_Order_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.LibraryVM.TagMng.SwitchOrdering();
        }

        #endregion //Tagペイン

        #region Informationペイン

        private void Information_Button_Close_Click(object sender, RoutedEventArgs e)
        {
            MainWindowVM.DisplayInformationPane = false;
        }

        private void Information_ListView_SelectedEntriesTags_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private async void Information_ListView_SelectedEntriesTags_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagCountViewModel)))
            {
                var imageTagCount = (TagCountViewModel)e.Data.GetData(typeof(TagCountViewModel));

                try
                {
                    await MainWindowVM.LibraryVM.TagMng.AddImageTagToSelectedObject(imageTagCount.Tag.Name);
                }
                catch (ArgumentException)
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        private async void Information_Button_TagPlus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MainWindowVM.LibraryVM.TagMng.AddImageTagToSelectedObject(TextBox_NewTag.Text);
            }
            catch (ArgumentException)
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private async void Information_Button_TagMinus_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = Information_ListView_SelectedEntriesTags.SelectedItems.Cast<Tag>().ToList();
            foreach (var selectedItem in selectedItems)
            {
                await MainWindowVM.LibraryVM.TagMng.RemoveImageTag(selectedItem.Name);
            }
        }

        private async void Information_ListView_SelectedEntriesTags_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listview = (ListView)sender;

            switch (e.Key)
            {
                case Key.Delete:
                    await MainWindowVM.LibraryVM.TagMng.RemoveImageTag(listview.SelectedItems.Cast<Tag>().Select(a => a.Name));
                    break;
                default:
                    break;
            }
        }

        #endregion //Informationペイン
    }
}
