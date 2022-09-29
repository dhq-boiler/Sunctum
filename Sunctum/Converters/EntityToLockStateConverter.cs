using Sunctum.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Sunctum.Converters
{
    public class EntityToLockStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var booleans = values.OfType<bool>().ToList();
            if (booleans.Count() != 2)
                return LockState.Collapsed;
            if (booleans[0] && !booleans[1])
                return LockState.Lock;
            else if (booleans[0] && booleans[1])
                return LockState.Unlock;
            return LockState.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
