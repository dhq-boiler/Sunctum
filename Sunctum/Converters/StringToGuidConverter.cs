

using System;
using System.Globalization;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class StringToGuidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid guid = (Guid)value;
            return guid.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid result;
            bool succeeded = Guid.TryParse(value as string, out result);
            if (succeeded) return result;
            else return Guid.Empty;
        }
    }
}
