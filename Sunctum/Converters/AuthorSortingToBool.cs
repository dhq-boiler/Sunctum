using Sunctum.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using Unity;

namespace Sunctum.Converters
{
    public class AuthorSortingToBool : IValueConverter
    {
        [Dependency]
        public Lazy<IAuthorPaneViewModel> AuthorPaneViewModel { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AuthorPaneViewModel.Value.SortingSelected(parameter as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public AuthorSortingToBool()
        { }
    }
}
