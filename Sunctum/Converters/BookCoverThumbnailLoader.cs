

using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.ViewModels;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class BookCoverThumbnailLoader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var book = value as BookViewModel;
            if (!book.ContentsRegistered)
            {
                Thread.Sleep(0);
            }
            if (!book.IsLoaded)
            {
                var task = Task.Run(() =>
                {
                    BookLoading.Load(book);
                });
                return new TaskCompletionSource<object>(task);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
