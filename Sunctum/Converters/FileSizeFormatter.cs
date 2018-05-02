

using Sunctum.Domain.Util;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class FileSizeFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return FileSize.ConvertFileSizeUnit((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
