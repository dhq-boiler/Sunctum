

using Ninject;
using NLog;
using Sunctum.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

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

        [Inject]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainWindowVM;
            bool ShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            await MainWindowVM.Initialize(true, ShiftPressed);
            HomeDocumentViewModel.BookSource = MainWindowVM.LibraryVM.LoadedBooks;
        }

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HomeDocumentViewModel.ResetScrollOffsetPool();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindowVM.Terminate();
        }
    }
}
