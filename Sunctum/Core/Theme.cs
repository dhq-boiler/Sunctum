

using Sunctum.Domain.ViewModels;
using Sunctum.UI.Controls;
using Sunctum.ViewModels;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sunctum.Core
{
    public partial class Theme : ResourceDictionary
    {
        public Theme()
        {
            InitializeComponent();
        }

        private void AutoScrollingHyperlink_HyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            var author = (hyperlink.DataContext as BookViewModel).Author;
            var viewModel = (DocumentViewModelBase)((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel).ActiveDocumentViewModel;
            viewModel.StoreScrollOffset(DocumentViewModelBase.BeforeSearchPosition);
            viewModel.SearchText = author.Name;
            viewModel.ResetScrollOffset();
        }
    }
}
