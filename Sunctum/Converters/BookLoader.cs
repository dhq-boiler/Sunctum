

using Sunctum.Domain.ViewModels;
using Sunctum.ViewModels;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class BookLoader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var book = value as BookViewModel;
            var viewModel = (App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel;
            viewModel.LibraryVM.FireFillContents(book);
            return book;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
