

using NLog;
using Sunctum.Domain.ViewModels;
using Sunctum.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using Unity;

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

        [Dependency]
        public IMainWindowViewModel MainWindowVM { get; set; }

        [Dependency]
        public IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainWindowVM;
            bool ShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            await MainWindowVM.Initialize(true, ShiftPressed);
        }

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HomeDocumentViewModel.ResetScrollOffsetPool();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindowVM.Terminate();
        }

        private void CommandBinding_CanExecute_Close(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_CanExecute_Maximize(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void CommandBinding_CanExecute_Minimize(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_CanExecute_Restore(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
    }
}
