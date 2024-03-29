﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class WindowStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "元に戻す")
            {
                switch ((WindowState)value)
                {
                    case WindowState.Maximized:
                    case WindowState.Minimized:
                        return Visibility.Visible;
                    case WindowState.Normal:
                        return Visibility.Collapsed;
                    default:
                        throw new NotSupportedException();
                }
            }
            else if (parameter.ToString() == "最大化")
            {
                switch ((WindowState)value)
                {
                    case WindowState.Maximized:
                        return Visibility.Collapsed;
                    case WindowState.Normal:
                    case WindowState.Minimized:
                        return Visibility.Visible;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                throw new Exception("parameter is not set");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
