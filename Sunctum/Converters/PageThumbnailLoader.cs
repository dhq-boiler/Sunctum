

using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sunctum.Converters
{
    [Obsolete]
    internal class PageThumbnailLoader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var page = value as PageViewModel;
            var task = Task.Run(async () =>
            {
                await ContentsLoadTask.Load(page).ConfigureAwait(false);
            });
            return new TaskCompletionSource<object>(task);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
