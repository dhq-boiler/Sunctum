

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class MarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => x.Equals(DependencyProperty.UnsetValue)))
            {
                return new Thickness(0, 0, 0, 0);
            }
            return new Thickness(
                System.Convert.ToDouble(values[0]),
                System.Convert.ToDouble(values[1]),
                System.Convert.ToDouble(values[2]),
                System.Convert.ToDouble(values[3])
            );
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
