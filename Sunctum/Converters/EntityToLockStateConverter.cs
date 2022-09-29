using Sunctum.Domain.ViewModels;
using Sunctum.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using YamlDotNet.Core.Tokens;

namespace Sunctum.Converters
{
    public class EntityToLockStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var booleans = values.Cast<bool>().ToList();
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
