using Microsoft.Practices.ServiceLocation;
using Sunctum.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using Unity;

namespace Sunctum.Converters
{
    public class BookSortingToBool : IValueConverter
    {
        [Dependency]
        public IHomeDocumentViewModel viewModel { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return viewModel?.SortingSelected(parameter as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public BookSortingToBool()
        {
        }
    }
}
