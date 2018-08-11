

using Sunctum.ViewModels;
using System;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (value is DocumentViewModelBase)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            if (value is DocumentViewModelBase)
                return value;

            return Binding.DoNothing;
        }
    }
}
