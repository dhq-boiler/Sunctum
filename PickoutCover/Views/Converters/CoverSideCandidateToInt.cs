

using PickoutCover.Domain.Logic.CoverSegment;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PickoutCover.Views.Converters
{
    internal class CoverSideCandidateToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = value as CoverSideCandidate;
            if (x != null)
            {
                return x.Offset;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var target = (int)value;
            return new CoverSideCandidate(target, false);
        }
    }
}
