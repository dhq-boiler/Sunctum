using Sunctum.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using Unity;

namespace Sunctum.Converters
{
    public class TagSortingToBool : IValueConverter
    {
        [Dependency]
        public Lazy<ITagPaneViewModel> TagPaneViewModel { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TagPaneViewModel.Value.SortingSelected(parameter as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public TagSortingToBool()
        { }
    }
}
